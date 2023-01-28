using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_BuildingHediffStats : CompProperties
    {
        public string stat;
        public float mod;
        public CompProperties_BuildingHediffStats()
        {
            compClass = typeof(BHStatComp);
        }
    }

}
