using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompNutritionConsumer : CompNutrition
    {
        private CompProperties_NutritionConsumer Props => (CompProperties_NutritionConsumer)props;
        public virtual float getConsumptionPerPulse()
        {
            return Props.consumptionPerPulse;
        }
    }
}