using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class BuildingBody
    {
        public CompBuildingCore heart = null;
        public CompScaffoldConverter scaffoldConverter = null;

        public HashSet<BuildingHediff> hediffs = new HashSet<BuildingHediff>();
        public HashSet<Building_Addiction> addictions = new HashSet<Building_Addiction>();
        public Dictionary<string, HashSet<CompAddictionSupplier>> addictionSuppliers = new Dictionary<string, HashSet<CompAddictionSupplier>>();
        public HashSet<CompScaffold> transformingScaff = new HashSet<CompScaffold>();
        public HashSet<Thing> bodyParts = new HashSet<Thing>();
        public HashSet<CompNutritionConsumer> consumers = new HashSet<CompNutritionConsumer>();
        public HashSet<CompNutritionStore> stores = new HashSet<CompNutritionStore>();
        public HashSet<CompNutritionSource> source = new HashSet<CompNutritionSource>();
        private int maxDepth = 5;
        public float currentNutrition = 0;
        public float nutritionCapacity = 0;
        public float passiveConsumption = 0;
        public float nutritionGen = 0;
        public bool dirty = false;

        public BodyOverlayDrawer Drawer
		{
			get
			{
				if (this.drawer == null)
				{
					Map currentMap = Find.CurrentMap;
                    Color bodyColor = new Color(Rand.Value, Rand.Value, Rand.Value);
					this.drawer = new BodyOverlayDrawer(this, bodyColor);
				}
				return this.drawer;
			}
		}
        private BodyOverlayDrawer drawer;

        public virtual string GetSpecies()
        {
            if (heart != null)
            {
                return heart.GetSpecies();
            }
            return null;
        }
        public virtual string GetName()
        {
            if (heart != null)
            {
                if (heart.bodyName != null)
                {
                    return heart.bodyName;
                } else
                {
                    return heart.bodyId;
                }
            }
            return "NA";
        }

        public virtual void Register(CompBuildingCore _heart)
        {
            heart = _heart;
            heart.body = this;
            foreach(BuildingHediff diff in _heart.hediffs)
            {
                hediffs.Add(diff);
                if (diff is Building_Addiction)
                {
                    addictions.Add((Building_Addiction)diff);
                }
            }
        }

        public virtual void Register(CompScaffoldConverter converter)
        {
            scaffoldConverter = converter;
            converter.body = this;
        }


        public virtual void Register(CompBuildingBodyPart comp)
        {
            if (comp is CompBuildingCore)
            {
                heart = (CompBuildingCore)comp;
                comp.body = this;
            } else
            {
                bodyParts.Add(comp.parent);
                comp.body = this;
            }
            foreach(BuildingHediff diff in comp.hediffs)
            {
                hediffs.Add(diff);
                if (diff is Building_Addiction)
                {
                    addictions.Add((Building_Addiction)diff);
                }
            }
            this.Drawer.SetDirty();
        }
        public virtual void Register(CompNutrition comp)
        {
            if (comp is CompNutritionConsumer)
            {
                consumers.Add((CompNutritionConsumer)comp);
                passiveConsumption += ((CompNutritionConsumer)comp).getConsumptionPerPulse();
            }
            if (comp is CompNutritionSource)
            {
                source.Add((CompNutritionSource)comp);
                nutritionGen += ((CompNutritionSource)comp).getNutritionPerPulse();
            }
            if (comp is CompNutritionStore)
            {
                stores.Add((CompNutritionStore)comp);
                nutritionCapacity += ((CompNutritionStore)comp).getNutrientCapacity();
                currentNutrition += ((CompNutritionStore)comp).getCurrentNutrition();
            }
            comp.body = this;
        }

        public virtual void Register(CompScaffold comp)
        {
            transformingScaff.Add(comp);
            comp.body = this;
        }

        public virtual void RegisterAddictionSupplier(CompAddictionSupplier addictionSupplier)
        {
            if (addictionSuppliers.TryGetValue(addictionSupplier.Props.addictionId, null) == null)
            {
                addictionSuppliers.Add(addictionSupplier.Props.addictionId, new HashSet<CompAddictionSupplier>());
            }
            addictionSuppliers
                .TryGetValue(addictionSupplier.Props.addictionId, new HashSet<CompAddictionSupplier>())
                .Add(addictionSupplier);
        }

        public virtual void DeRegister(CompBuildingBodyPart comp)
        {
            bodyParts.Remove(comp.parent);
            drawer.SetDirty();
            this.dirty = true;
            //comp.body = null;
        }
        public virtual void DeRegister(CompNutrition comp)
        {
            if (comp is CompNutritionConsumer)
            {
                consumers.Remove((CompNutritionConsumer)comp);
                passiveConsumption -= ((CompNutritionConsumer)comp).getConsumptionPerPulse();
            }
            if (comp is CompNutritionStore)
            {
                stores.Remove((CompNutritionStore)comp);
                nutritionCapacity -= ((CompNutritionStore)comp).getNutrientCapacity();
                currentNutrition -= ((CompNutritionStore)comp).getNutrientCapacity();
            }
            if (comp is CompNutritionSource)
            {
                source.Add((CompNutritionSource)comp);
                nutritionGen += ((CompNutritionSource)comp).getNutritionPerPulse();
            }
        }

        public virtual void DeRegister(CompScaffold scaff)
        {
            transformingScaff.Remove(scaff);
            scaff.bodyId = null;
            scaff.body = null;
            scaff.converter = null;
            scaff.transforming = false;
        }

        public virtual void DeRegisterAllBodyparts(DestroyMode mode, Map previousMap)
        {
            Thing[] bodyPartsCopy = new Thing[this.bodyParts.Count];
            this.bodyParts.CopyTo(bodyPartsCopy);
            foreach(Thing t in bodyPartsCopy)
            {
                t.TryGetComp<CompBuildingBodyPart>()?.Detatch(mode, previousMap);
            }
        }

        public virtual void DeRegisterAllScaffolds()
        {
            foreach(CompScaffold scaff in this.transformingScaff)
            {
                scaff.bodyId = null;
                scaff.body = null;
                scaff.converter = null;
                scaff.transforming = false;
            }
            this.transformingScaff = new HashSet<CompScaffold>();
        }

        public virtual void TerminateBody(DestroyMode mode, Map previousMap)
        {
            DeRegisterAllScaffolds();
            DeRegisterAllBodyparts(mode, previousMap);
            this.heart = null;
        }

        public virtual void DeRegisterAddictionSupplier(CompAddictionSupplier addictionSupplier)
        {
            addictionSuppliers.TryGetValue(addictionSupplier.Props.addictionId, new HashSet<CompAddictionSupplier>())
                .Remove(addictionSupplier);
        }

        public virtual void UpdateNutritionGeneration()
        {
            nutritionGen = 0;
            foreach(CompNutritionSource c in source)
            {
                nutritionGen += c.getNutritionPerPulse();
            }
        }
        public virtual void UpdateCurrentNutrition()
        {
            nutritionCapacity = 0;
            currentNutrition = 0;
            foreach (CompNutritionStore c in stores)
            {
                nutritionCapacity += c.getNutrientCapacity();
                currentNutrition += c.getCurrentNutrition();
            }
        }
        public virtual void UpdatePassiveConsumption()
        {
            float metabolicOffset = 1f;
            if (heart != null)
            {
                metabolicOffset = heart.GetStat("metabolcEfficiency");
            }
            passiveConsumption = heart.GetPassiveConsumptions();
            foreach (CompNutritionConsumer c in consumers)
            {
                passiveConsumption += c.getConsumptionPerPulse();
            }
        }
        public virtual bool RequestNutrition(float qty)
        {
            if (qty > currentNutrition)
            {
                return false;
            }

            ExtractNutrition(stores, qty, 0);
            return true;
        }

        public virtual void RunNutrition(float ticks)
        {
            if (heart == null)
            {
                return;
            }
            if (heart.hungerDuration > heart.hungerThreshold && currentNutrition <= 0)
            {
                heart.DoHunger();
            }
            UpdateCurrentNutrition();
            UpdatePassiveConsumption();
            UpdateNutritionGeneration();
            float net = nutritionGen - passiveConsumption;
            net = net / (60000f/ticks);
            if (net > 0)
            {
                heart.hungerDuration = 0;
                float toStore = net * 0.5f;
                float leftover = 0;
                if ((nutritionCapacity - currentNutrition) <= 0)
                {
                    leftover = toStore;
                } 
                else if (toStore >= (nutritionCapacity - currentNutrition)) 
                {
                    leftover = toStore - (nutritionCapacity - currentNutrition);
                    currentNutrition = nutritionCapacity;
                    foreach (CompNutritionStore store in stores)
                    {
                        store.currentNutrition = store.getNutrientCapacity();
                    }
                } else
                {
                    leftover = StoreNutrition(stores, toStore, 0);
                }
            }
            if (net <= 0)
            {
                net = net * -1;
                if (net > this.currentNutrition)
                {
                    currentNutrition = 0;
                    foreach (CompNutritionStore store in stores)
                    {
                        store.currentNutrition = 0;
                    }
                    heart.hungerDuration += ticks;
                }
                else
                {
                    ExtractNutrition(stores, net, 0);
                }
            }
        }

        public virtual float StoreNutrition(HashSet<CompNutritionStore> _stores, float toStore, int depth)
        {
            if (_stores.Count == 0 || depth > maxDepth)
            {
                return toStore;
            }
            float leftOver = 0;
            HashSet<CompNutritionStore> retainCapactiy = new HashSet<CompNutritionStore>();
            float storeEach = toStore/_stores.Count;
            foreach (CompNutritionStore s in _stores)
            {
                leftOver += s.storeNutrition(storeEach);
                if (s.currentNutrition < s.getNutrientCapacity())
                {
                    retainCapactiy.Add(s);
                }
            }

            if (leftOver <= 0)
            {
                return 0;
            } 
            else
            {
                return StoreNutrition(retainCapactiy, leftOver, depth+1);
            }
        }
        public virtual float ExtractNutrition(HashSet<CompNutritionStore> _stores, float toExtract, int depth)
        {
            if (_stores.Count <= 0 || depth > maxDepth)
            {
                return toExtract;
            }
            HashSet<CompNutritionStore> retainNutrition = new HashSet<CompNutritionStore>();
            float localExtract = toExtract/_stores.Count;
            float remainingHunger = 0;
            foreach (CompNutritionStore s in _stores)
            {
                remainingHunger += s.consumeNutrition(localExtract);
                if (s.currentNutrition > 0)
                {
                    retainNutrition.Add(s);
                }
            }
            if (remainingHunger <= 0 || retainNutrition.Count == 0)
            {
                return 0;
            } else
            {
                return ExtractNutrition(retainNutrition, remainingHunger, depth+1);
            }
        }

        public virtual void RunAddictions(float ticks)
        {
            foreach (Building_Addiction addiction in addictions)
            {
                HashSet<CompAddictionSupplier> suppliers = new HashSet<CompAddictionSupplier>();
                foreach(CompAddictionSupplier supplier in addictionSuppliers.TryGetValue(addiction.def.defName, new HashSet<CompAddictionSupplier>()))
                {
                    if (supplier.CanSupply())
                    {
                        suppliers.Add(supplier);
                    } else
                    {
                        supplier.SetConsumption(0f);
                    }
                }
                if (suppliers.Count > 0)
                {
                    addiction.withdrawl = 0;
                    float consumptionRate = (addiction.massMult * bodyParts.Count)/suppliers.Count;
                    foreach(CompAddictionSupplier supplier in suppliers)
                    {
                        supplier.SetConsumption(consumptionRate);
                    }
                } else
                {
                    addiction.withdrawl += addiction.withdrawRate * ticks;
                }
            }
        }

        public virtual void BodyDetectionPulse()
        {
            if (!this.dirty || this.heart == null)
            {
                return;
            }
            Map coreMap = this.heart.parent.Map;
            HashSet<Thing> seenBodyParts = new HashSet<Thing>();
            HashSet<Thing> detatchedParts = new HashSet<Thing>();
            seenBodyParts.Add(this.heart.parent);
            foreach(IntVec3 footprint in GenAdj.CellsOccupiedBy(this.heart.parent))
            {
                foreach(Thing t in footprint.GetThingList(coreMap))
                {
                    if (t.TryGetComp<CompBuildingBodyPart>()?.bodyId == this.heart.bodyId)
                    {
                        seenBodyParts.Add(t);
                        RecursiveSeen(t, ref seenBodyParts);
                    }
                }
            }
            foreach(Thing t in bodyParts.Where((s) => !seenBodyParts.Contains(s)))
            {
                detatchedParts.Add(t);
            }
            foreach(Thing t in detatchedParts)
            {
                t.TryGetComp<CompBuildingBodyPart>()?.Detatch(DestroyMode.KillFinalize, null);
            }
            HashSet<CompScaffold> removeScaff = new HashSet<CompScaffold>();
            foreach (CompScaffold scaff in this.transformingScaff)
            {
                bool remove = true;
                foreach (IntVec3 adj in GenAdj.CellsAdjacentCardinal(scaff.parent))
                {
                    foreach(Thing t in adj.GetThingList(scaff.parent.Map))
                    {
                        if (t.TryGetComp<CompBuildingBodyPart>()?.bodyId == scaff.bodyId)
                        {
                            remove = false;
                            break;
                        }
                    }
                    if (remove)
                    {
                        removeScaff.Add(scaff);
                    }
                }
            }
            foreach (CompScaffold scaff in removeScaff)
            {
                this.DeRegister(scaff);
            }
            this.scaffoldConverter?.CullToConvert();
            this.dirty = false;
        }

        public virtual void RecursiveSeen(Thing toCheck, ref HashSet<Thing> seen)
        {
            foreach(IntVec3 adj in GenAdj.CellsAdjacentCardinal(toCheck)) {
                foreach(Thing t in adj.GetThingList(toCheck.Map))
                {
                    if (!seen.Contains(t) && t.TryGetComp<CompBuildingBodyPart>()?.bodyId == this.heart.bodyId)
                    {
                        seen.Add(t);
                        RecursiveSeen(t, ref seen);
                    }
                }
            }
        }
    }
}