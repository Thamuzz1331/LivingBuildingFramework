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
        public List<Hediff_Building> hediffs = new List<Hediff_Building>();

        public virtual bool HeartSpawned
        {
            get
            {
                return (this.body != null && this.body.heart != null);
            }
        }

        public virtual bool VisibleHediffs
        {
            get
            {
                return hediffs.Any(diff => diff.visible);
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
            Scribe_Collections.Look<Hediff_Building>(ref hediffs, "hediffs", LookMode.Deep);
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
            foreach(Hediff_Building diff in hediffs)
            {
                if (diff.visible)
                {
                    b.Append("\nHediff " + diff.ToString());
                }
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

        public virtual void AddHediff(Hediff_Building toAdd)
        {
            toAdd.bodyPart = parent;
            this.hediffs.Add(toAdd);
            toAdd.PostAdd();
            body.hediffs.Add(toAdd);
        }

        public virtual Hediff_Building TryGetHediff<T>(Hediff_Building failVal)
        {
            Hediff_Building ret = hediffs.Find(diff => diff is T);
            if (ret == null)
            {
                return failVal;
            }
            return ret;
        }

        public virtual void RemoveHediff(Hediff_Building b)
        {
            hediffs.Remove(b);
        }

        public virtual void RemoveHediff(string removeLabel)
        {
            hediffs = hediffs.FindAll(diff => !(diff.label == removeLabel));
        }

        public virtual void RemoveHediff<T>()
        {
            hediffs = hediffs.FindAll(diff => !(diff is T));
        }

    }
}