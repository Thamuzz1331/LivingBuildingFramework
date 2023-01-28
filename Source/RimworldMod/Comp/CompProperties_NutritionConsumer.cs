using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using LivingBuildings;

namespace RimWorld
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
