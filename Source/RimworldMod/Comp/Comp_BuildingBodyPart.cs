using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompBuildingBodyPart : ThingComp
    {
        public CompProperties_BuildingBodyPart Props => (CompProperties_BuildingBodyPart)props;
        private List<Thing> scaffolds = new List<Thing>();

        public String bodyId = "NA";
        public BuildingBody body = null;
        public List<BuildingHediff> hediffs = new List<BuildingHediff>();

        public virtual bool CoreSpawned
        {
            get
            {
                return (this.body != null && this.body.heart != null);
            }
        }

        public virtual CompBuildingCore Core
        {
            get
            {
                if (this.body == null)
                {
                    return null;
                }
                return this.body.heart;
            }
        }

        public virtual bool VisibleHediffs
        {
            get
            {
                return hediffs.Any(diff => diff.Visible);
            }
        }

        public void SetId(String _id)
        {
            bodyId = _id;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref bodyId, "bodyId", "NA");
            Scribe_Collections.Look<BuildingHediff>(ref hediffs, "hediffs", LookMode.Deep);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
            foreach (IntVec3 r in GenAdj.CellsOccupiedBy(parent))
            {
                List<IntVec3> adjSpaces = GenAdjFast.AdjacentCellsCardinal(r);
                //adjSpaces = adjSpaces.OrderBy(a => Rand.Range(0, 100)).ToList();
                foreach (IntVec3 c in adjSpaces)
                {
                    foreach (Thing adj in c.GetThingList(parent.Map))
                    {
                        CompScaffold scaff = adj.TryGetComp<CompScaffold>();
                        if (scaff != null && !scaff.transforming && scaff.GetSpecies() == this.GetSpecies())
                        {
                            AddScaff(adj);
                            return;
                        }
                    }
                }
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (body != null)
                body.DeRegister(this);
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder b = new StringBuilder(base.CompInspectStringExtra());
            if (body != null && body.heart != null)
            {
                b.Append("Flesh of " + body.GetName());
            }
            return b.ToString();
        }

        public virtual void AddScaff(Thing scaff)
        {
            if (this.body != null && this.body.heart != null)
            {
                CompScaffoldConverter converter = body.scaffoldConverter;
                if (converter != null)
                {
                    converter.AddToConvert(scaff);
                    return;
                }
            } else
            {
                scaffolds.Add(scaff);
            }
        }
        public virtual List<Thing> GetScaff()
        {
            return scaffolds;
        }
        public virtual void ClearScaff()
        {
            scaffolds.Clear();
        }
        public virtual string GetSpecies()
        {
            return Props.species;
        }

        public virtual void AddHediff(BuildingHediff toAdd)
        {
            toAdd.bodyPart = parent;
            toAdd.bp = this;
            this.hediffs.Add(toAdd);
            toAdd.PostAdd();
            body.hediffs.Add(toAdd);
        }

        public virtual BuildingHediff TryGetHediff<T>(BuildingHediff failVal)
        {
            BuildingHediff ret = hediffs.Find(diff => diff is T);
            if (ret == null)
            {
                return failVal;
            }
            return ret;
        }

        public virtual void RemoveHediff(BuildingHediff b)
        {
            hediffs.Remove(b);
            b.PostRemove();
        }

        public virtual void RemoveHediff(string removeLabel)
        {
            List<BuildingHediff> toRemove = hediffs.FindAll(diff => (diff.DisplayLabel == removeLabel));
            foreach(BuildingHediff diff in toRemove)
            {
                RemoveHediff(diff);
            }
        }

        public virtual void RemoveHediff<T>()
        {
            List<BuildingHediff> toRemove = hediffs.FindAll(diff => (diff is T));
            foreach(BuildingHediff diff in toRemove)
            {
                RemoveHediff(diff);
            }
        }

    }
}