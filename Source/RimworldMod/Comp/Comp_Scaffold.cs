using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace LivingBuildings
{
    public class CompScaffold : ThingComp
    {
        public CompProperties_Scaffold Props => (CompProperties_Scaffold)props;

        public bool transforming = false;
        public float transformCountdown;
        public String bodyId;
        public BuildingBody body = null;
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
            Scribe_Values.Look<string>(ref bodyId, "bodyId");
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
            }else
            {
                ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
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

        public virtual Thing Convert(CompScaffoldConverter converter, bool instant = false)
        {
            if (transforming)
            {
                return null;
            }
            transformDef = this.GetConversionDef(converter);
            if (transformDef != null)
            {
                this.bodyId = converter.parent.TryGetComp<CompBuildingBodyPart>().bodyId;
                converter.parent.TryGetComp<CompBuildingBodyPart>().body.Register(this);
                this.converter = converter;
                if (instant)
                {
                    return MakeReplacement(transformDef, converter);
                }
                this.transformCountdown = Props.transformTime;
                transforming = true;
                return parent;
                //return MakeReplacement(replacementDef, converter);
            }
            return null;
        }

        public virtual void BodyTick()
        {
            if (transforming)
            {
                if (transformCountdown <= 0)
                {
                    if (converter == null)
                    {
                        converter = body.scaffoldConverter;
                    }
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