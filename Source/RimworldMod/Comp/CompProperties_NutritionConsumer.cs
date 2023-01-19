using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace LivingBuildings
{
    public class CompProperties_NutritionConsumer : CompProperties
    {
        public float consumptionPerPulse;
        public CompProperties_NutritionConsumer()
        {
            compClass = typeof(CompNutritionConsumer);
        }

    }
}
