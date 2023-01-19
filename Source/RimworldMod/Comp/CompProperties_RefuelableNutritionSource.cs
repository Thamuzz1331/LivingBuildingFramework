using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace LivingBuildings
{
    public class CompProperties_RefuelableNutritionSource : CompProperties_NutritionSource
    {
        public CompProperties_RefuelableNutritionSource()
        {
            compClass = typeof(CompRefuelableNutritionSource);
        }

    }
}
