using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace RimWorld
{
    public class Dialog_NameBody : Dialog_Rename
    {
        private CompBuildingCore core;

        public Dialog_NameBody(CompBuildingCore b)
        {
            this.core = b;
            curName = b.bodyName;
        }

        protected override void SetName(string name)
        {
            if (name == core.bodyName || string.IsNullOrEmpty(name))
                return;

            core.bodyName = name;
        }
    }
}