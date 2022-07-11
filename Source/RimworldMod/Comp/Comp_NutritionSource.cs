using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompNutritionSource : CompNutrition
    {
        public CompProperties_NutritionSource Props => (CompProperties_NutritionSource)props;
        public virtual float getNutritionPerPulse()
        {
            return Props.nutrientPerPulse;
        }
    }
}