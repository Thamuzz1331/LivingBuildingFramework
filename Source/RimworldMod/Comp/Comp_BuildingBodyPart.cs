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
        public List<IHediff> hediffs = new List<IHediff>();

        public void SetId(String _id)
        {
            bodyId = _id;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref bodyId, "bodyId", "NA");
            Scribe_Collections.Look<IHediff>(ref hediffs, "hediffs", LookMode.Deep);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if(respawningAfterLoad)
            {
                foreach (IHediff hediff in hediffs)
                {
                    hediff.Apply(this);
                }
            }
            ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
            foreach (IntVec3 r in GenAdj.CellsOccupiedBy(parent))
            {
                List<IntVec3> adjSpaces = GenAdjFast.AdjacentCells8Way(r);
                //adjSpaces = adjSpaces.OrderBy(a => Rand.Range(0, 100)).ToList();
                foreach (IntVec3 c in adjSpaces)
                {
                    foreach (Thing adj in c.GetThingList(parent.Map))
                    {
                        CompScaffold scaff = adj.TryGetComp<CompScaffold>();
                        if (scaff != null && scaff.GetSpecies() == this.GetSpecies())
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
            foreach(IHediff diff in hediffs)
            {
                b.Append("\nHediff " + diff.ToString());
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

    }
}