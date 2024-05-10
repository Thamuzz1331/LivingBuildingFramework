using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompBuildingCore : CompBuildingBodyPart, IRenameable
    {
        public CompProperties_BuildingCore CoreProps => (CompProperties_BuildingCore)props;
        public string bodyName = "nameless thing";
        public float hungerDuration = 0;
        public float hungerThreshold = 300f;
        public int geneMetabolicCost = 0;
        public List<BuildingGene> genes = new List<BuildingGene>();

        string IRenameable.RenamableLabel { 
            get {
                return bodyName;
            }
            set
            {
                bodyName = value;
            } 
        }
		string IRenameable.BaseLabel { 
            get
            {
                return bodyName;
            } 
        }
		string IRenameable.InspectLabel { 
            get
            {
                return bodyName;
            } 
        }

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
            Scribe_Values.Look<string>(ref bodyName, "bodyName", "nameless thing");
            Scribe_Values.Look<int>(ref geneMetabolicCost, "geneMetabolicCost", 0);
            Scribe_Collections.Look<BuildingGene>(ref genes, "genes", LookMode.Deep);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                if (bodyId == "NA" && parent.Faction == Faction.OfPlayer)
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

        public override void DoRegister()
        {
            ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).RegisterCore(this);
        }

        public virtual void AddGene(BuildingGene b)
        {
            genes.Add(b);
            this.geneMetabolicCost += b.def.metabolicCost;
            b.PostAdd(this);
        }

        public virtual void RemoveGene(BuildingGene b)
        {
            genes.Remove(b);
            this.geneMetabolicCost -= b.def.metabolicCost;
            b.PostRemove(this);
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            ((MapCompBuildingTracker)previousMap.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Terminate(this.bodyId, mode, previousMap);
            base.PostDestroy(mode, previousMap);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            Command_Action renameBody = new Command_Action
            {
                action = delegate
                {
                    Find.WindowStack.Add(new Dialog_NameBody(this));
                },
                hotKey = KeyBindingDefOf.Misc1,
                icon = ContentFinder<Texture2D>.Get("UI/Commands/RenameZone"),
                defaultLabel = TranslatorFormattedStringExtensions.Translate("BodyRename"),
                defaultDesc = TranslatorFormattedStringExtensions.Translate("BeodyRenameDesc")
            };
            yield return renameBody;
            foreach (BuildingGene g in genes)
            {
                foreach (Gizmo genegizmo in g.GeneGetGizmosExtra())
                {
                    yield return genegizmo;
                }
            }
        }

        public virtual void SetName(string name)
        {
            this.bodyName = name;
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
                case "metabolicEfficiency" :
                    return ret * GetMetbolicCost();
                case "growthEfficiency":
                    return ret * GetStat("metabolicEfficiency");
                case "growthSpeed":
                    return ret * GetStat("metabolicSpeed");
                default:
                    return ret;
            }
        }

        private float GetMetbolicCost()
        {
            return 1f + (0.1f * geneMetabolicCost);
        }

        public virtual float GetMultiplier(string mult)
        {
            return multipliers.TryGetValue(mult, 1f);
        }

        public virtual float GetPassiveConsumptions()
        {

            float metabolicOffset = 1f;
            return (0.025f*(body.bodyParts.Count + 25))/metabolicOffset;
        }

    }
}