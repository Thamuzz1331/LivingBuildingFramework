using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompBuildingCore : CompBuildingBodyPart
    {
        public CompProperties_BuildingCore CoreProps => (CompProperties_BuildingCore)props;
        public string bodyName = null;
        public float hungerDuration = 0;

        public Dictionary<string, float> stats = new Dictionary<string, float>();

        public override void PostExposeData()
        {
            Scribe_Values.Look<String>(ref bodyId, "bodyId", null);
            Scribe_Values.Look<String>(ref bodyId, "bodyId", null);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                bodyId = Guid.NewGuid().ToString();
                if (parent.TryGetComp<CompScaffoldConverter>() != null)
                {
                    parent.TryGetComp<CompScaffoldConverter>().bodyId=bodyId;
                    parent.TryGetComp<CompScaffoldConverter>().DetectionPulse();
                    ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompScaffoldConverter>());
                }
            }
            ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);

        }

        public virtual float GetStat(string stat)
        {
            return stats.TryGetValue(stat, 0f);
        }
        public virtual float GetMultiplier(string stat)
        {
            return 1f;
        }
    }
}