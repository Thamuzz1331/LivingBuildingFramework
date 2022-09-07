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

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            foreach (IntVec3 r in GenAdj.CellsOccupiedBy(parent))
            {
                foreach (IntVec3 c in GenAdjFast.AdjacentCells8Way(r))
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
            ThingDef replacementDef = this.GetConversionDef(converter);
            if (replacementDef != null)
            {
                return MakeReplacement(replacementDef, converter);
            }
            return null;
        }

        public virtual string GetSpecies()
        {
            return this.Props.species;
        }
    }
}