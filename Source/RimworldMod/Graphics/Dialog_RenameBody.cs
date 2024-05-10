using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimWorld
{
    public class Dialog_NameBody : Dialog_Rename<CompBuildingCore>
    {
        private CompBuildingCore core;

        public Dialog_NameBody(CompBuildingCore b) : base(b)
        {
            this.core = b;
            curName = b.bodyName;
        }

    }
}