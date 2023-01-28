using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;

namespace RimWorld
{
    public class CompRefuelableNutritionSource : CompNutritionSource
    {
        public CompProperties_RefuelableNutritionSource RefuelableProps => (CompProperties_RefuelableNutritionSource)props;
        public override float getNutritionPerPulse()
        {
            if (parent.TryGetComp<CompRefuelable>() != null && parent.TryGetComp<CompRefuelable>().Fuel <= 0)
            {
                return 0;
            }
            return Props.nutrientPerPulse;
        }
    }
}