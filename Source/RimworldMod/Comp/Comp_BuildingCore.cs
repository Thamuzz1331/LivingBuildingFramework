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
        public string bodyName = "nameless thing";
        public float hungerDuration = 0;
        public float hungerThreshold = 300f;

        public Dictionary<string, float> stats = new Dictionary<string, float>() {
            {"conversionCost", 15f}
        };
        public Dictionary<string, float> multipliers = new Dictionary<string, float>();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref bodyId, "bodyId", null);
            Scribe_Values.Look<float>(ref hungerDuration, "hungerDuration", 300f);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                if (bodyId == "NA")
                    bodyId = Guid.NewGuid().ToString();
                if (parent.TryGetComp<CompScaffoldConverter>() != null)
                {
                    parent.TryGetComp<CompScaffoldConverter>().bodyId=bodyId;
                }
            }
            ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
            if (!respawningAfterLoad)
            {
                ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompScaffoldConverter>());
                parent.TryGetComp<CompScaffoldConverter>().DetectionPulse();
            }

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if ((int)parent.GetStatValue(inducers) > 0)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Set Body Name",
                    action = delegate ()
                    {
                        this.SetName();
                    }
                };
            }

        }

        public virtual void SetName()
        {

        }

        public virtual void DoHunger()
        {

        }
        public virtual float GetStat(string stat)
        {
            return stats.TryGetValue(stat, 0f);
        }
        public virtual float GetMultiplier(string mult)
        {
            return multipliers.TryGetValue(mult, 1f);
        }
    }
}