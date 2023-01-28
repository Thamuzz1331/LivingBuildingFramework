using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_AddictionSupplier : CompProperties
    {
        public string addictionId;
        public CompProperties_AddictionSupplier()
        {
            compClass = typeof(CompAddictionSupplier);
        }
    }

}
