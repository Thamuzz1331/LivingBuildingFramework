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
                        if (adj is ThingWithComps)
                        {
                            CompBuildingBodyPart flesh = ((ThingWithComps)adj).TryGetComp<CompBuildingBodyPart>();
                            if (flesh != null && flesh.Props.species == this.Props.species)
                            {
                                flesh.AddScaff(parent);
                            }
                        }
                    }
                }
            }
        }

		public virtual ThingDef GetConversionDef()
        {
			return ThingDef.Named(Props.transformString);
        }

        public virtual Thing Convert(CompScaffoldConverter converter)
        {
            ThingDef replacementDef = this.GetConversionDef();

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
			replacement.SetFaction(Faction.OfPlayer);
            IntVec3 c = parent.Position;
			TerrainDef terrain = parent.Map.terrainGrid.TerrainAt(c);
			parent.Map.terrainGrid.RemoveTopLayer(c, false);
			parent.Destroy();
			replacement.SpawnSetup(parent.Map, false);
            return replacement;
        }
    }
}