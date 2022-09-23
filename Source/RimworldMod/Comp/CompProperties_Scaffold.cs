using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorld
{
    public class CompProperties_Scaffold : CompProperties
    {
        public string species = null;
        public string transformString = null;
        public float transformTime = 300f;

        public CompProperties_Scaffold()
        {
            compClass = typeof(CompScaffold);
        }
    }

}
