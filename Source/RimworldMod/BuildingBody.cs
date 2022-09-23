using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;
using LivingBuilding;

namespace RimWorld
{
    public class BuildingBody
    {
        public CompBuildingCore heart = null;
        public CompScaffoldConverter scaffoldConverter = null;

        public HashSet<Hediff_Building> hediffs = new HashSet<Hediff_Building>();
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

        public LivingBuilding.BodyOverlayDrawer Drawer
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

        public virtual void DeRegister(CompBuildingBodyPart comp)
        {
            bodyParts.Remove(comp.parent);
            drawer.SetDirty();
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
            passiveConsumption = (0.05f*(bodyParts.Count + 150))/metabolicOffset;
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
            if (heart.hungerDuration > heart.hungerThreshold)
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
            if (net < 0)
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
    }
}