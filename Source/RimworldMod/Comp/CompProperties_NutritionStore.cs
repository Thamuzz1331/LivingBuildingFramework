using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_NutritionStore : CompProperties
    {
        public float nutrientCapacity;
        public float initialNutrition;
        public CompProperties_NutritionStore()
        {
            compClass = typeof(CompNutritionStore);
        }
    }
}
