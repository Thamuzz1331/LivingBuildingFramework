using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_BuildingCore : CompProperties_BuildingBodyPart
    {
        public CompProperties_BuildingCore()
        {
            compClass = typeof(CompBuildingCore);
        }
    }

}
