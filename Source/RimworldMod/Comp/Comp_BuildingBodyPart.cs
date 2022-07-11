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
        public List<IHeaddiff> headdiffs = new List<IHeaddiff>();

        public void SetId(String _id)
        {
            bodyId = _id;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<String>(ref bodyId, "bodyId", "NA");
            Scribe_Collections.Look<IHeaddiff>(ref headdiffs, "headdiffs");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if(respawningAfterLoad)
            {
                foreach (IHeaddiff headdiff in headdiffs)
                {
                    headdiff.Apply(this);
                }
            }
            ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
            foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(parent.Position))
            {
                foreach (Thing adj in c.GetThingList(parent.Map))
                {
                    if (adj is ThingWithComps)
                    {
                        CompScaffold scaff = ((ThingWithComps)adj).TryGetComp<CompScaffold>();
                        if (scaff != null)
                        {
                            AddScaff(scaff.parent);
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
            if (body != null && body.heart != null)
            {
                return "Flesh of " + bodyId;
            }
            return "";
        }

        public virtual void AddScaff(Thing scaff)
        {
            if (this.body != null && this.body.heart != null)
            {
                CompScaffoldConverter converter = body.scaffoldConverter;
                if (converter != null)
                {
                    converter.toConvert.Enqueue(scaff);
                    return;
                }
            }
            scaffolds.Add(scaff);
        }
        public virtual List<Thing> GetScaff()
        {
            return scaffolds;
        }
        public virtual void ClearScaff()
        {
            scaffolds.Clear();
        }
    }
}