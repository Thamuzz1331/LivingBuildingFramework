using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    public class CompProperties_BuildingBodyPart : CompProperties
    {
        public string species = null;
        public CompProperties_BuildingBodyPart()
        {
            compClass = typeof(CompBuildingBodyPart);
        }
    }

}
