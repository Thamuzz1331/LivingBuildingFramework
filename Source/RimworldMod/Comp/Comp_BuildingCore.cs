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
        public List<BuildingGene> genes = new List<BuildingGene>();

        public Dictionary<string, float> stats = new Dictionary<string, float>() {
            {"metabolicEfficiency", 1f},
            {"metabolicSpeed", 1f},
            {"growthEfficiency", 1f},
            {"growthSpeed", 1f},
        };
        public Dictionary<string, float> multipliers = new Dictionary<string, float>();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref bodyId, "bodyId", null);
            Scribe_Values.Look<float>(ref hungerDuration, "hungerDuration", 300f);
            Scribe_Collections.Look<BuildingGene>(ref genes, "genes");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                if (bodyId == "NA")
                    bodyId = Guid.NewGuid().ToString();
                if (parent.TryGetComp<CompScaffoldConverter>() != null)
                {
                    parent.TryGetComp<CompScaffoldConverter>().bodyId=bodyId;
                }
            }
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                ((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompScaffoldConverter>());
                parent.TryGetComp<CompScaffoldConverter>().DetectionPulse();
            }
            foreach(BuildingGene g in genes)
            {
                g.PostSpawnSetup(respawningAfterLoad);
            }
        }

        public virtual void AddGene(BuildingGene b)
        {
            genes.Add(b);
            b.PostAdd(this);
        }

        public virtual void RemoveGene(BuildingGene b)
        {
            genes.Remove(b);
            b.PostRemove(this);
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
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
            float ret = stats.TryGetValue(stat, 1f);
            foreach (BuildingHediff diff in hediffs)
            {
                ret *= diff.StatMod(stat);
            }

            switch (stat)
            {
                case "growthEfficiency":
                    return ret * GetStat("metabolicEfficiency");
                case "growthSpeed":
                    return ret * GetStat("metabolicSpeed");
                default:
                    return ret;
            }
        }

        public virtual float GetMultiplier(string mult)
        {
            return multipliers.TryGetValue(mult, 1f);
        }
    }
}