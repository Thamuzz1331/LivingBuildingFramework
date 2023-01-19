using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace LivingBuildings
{
    public class CompProperties_NutritionSource : CompProperties
    {
        public float nutrientPerPulse;
        public CompProperties_NutritionSource()
        {
            compClass = typeof(CompNutritionSource);
        }

    }
}
