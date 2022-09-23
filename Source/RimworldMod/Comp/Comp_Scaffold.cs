using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompScaffold : ThingComp
    {
        public CompProperties_Scaffold Props => (CompProperties_Scaffold)props;

        public bool transforming = false;
        public float transformCountdown;
        public CompScaffoldConverter converter;
        public ThingDef transformDef;

        public virtual float TransformTime
        {
            get
            {
                if (converter != null)
                {
                    return Props.transformTime / converter.body.heart.GetStat("metabolicSpeed");
                }
                return Props.transformTime; 
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref transforming, "transforming", false);
            Scribe_Values.Look<float>(ref transformCountdown, "transformCountdown", 0f);
            Scribe_Values.Look<CompScaffoldConverter>(ref converter, "converter", null);
            Scribe_Defs.Look(ref transformDef, "transformDef");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!transforming)
            {
                foreach (IntVec3 r in GenAdj.CellsOccupiedBy(parent))
                {
                    foreach (IntVec3 c in GenAdjFast.AdjacentCellsCardinal(r))
                    {
                        foreach (Thing adj in c.GetThingList(parent.Map))
                        {
                            CompBuildingBodyPart flesh = adj.TryGetComp<CompBuildingBodyPart>();
                            if (flesh != null && flesh.Props.species == this.Props.species)
                            {
                                flesh.AddScaff(parent);
                            }
                        }
                    }
                }
            }
        }

		public virtual ThingDef GetConversionDef(CompScaffoldConverter converter)
        {
			return ThingDef.Named(Props.transformString);
        }

        public virtual Thing MakeReplacement(ThingDef replacementDef, CompScaffoldConverter converter)
        {
            Thing replacement = ThingMaker.MakeThing(replacementDef);
			CompBuildingBodyPart bodyPart = ((ThingWithComps)replacement).GetComp<CompBuildingBodyPart>();
			if(bodyPart != null)
			{
				bodyPart.SetId(converter.bodyId);
			}
			CompNutrition nutrition = ((ThingWithComps)replacement).GetComp<CompNutrition>();
			if (nutrition != null)
			{
				nutrition.SetId(converter.bodyId);
			}
			replacement.Rotation = parent.Rotation;
			replacement.Position = parent.Position;
			replacement.SetFaction(parent.Faction);
			parent.Destroy();
			replacement.SpawnSetup(converter.parent.Map, false);
            return replacement;
        }

        public virtual Thing Convert(CompScaffoldConverter converter)
        {
            if (transforming)
            {
                return null;
            }
            transformDef = this.GetConversionDef(converter);
            if (transformDef != null)
            {
                this.converter = converter;
                this.transformCountdown = Props.transformTime;
                transforming = true;
                return parent;
                //return MakeReplacement(replacementDef, converter);
            }
            return null;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (transforming)
            {
                if (transformCountdown <= 0)
                {
                    MakeReplacement(transformDef, converter);
                }
                transformCountdown--;
            }
        }

        public virtual string GetSpecies()
        {
            return this.Props.species;
        }
    }
}