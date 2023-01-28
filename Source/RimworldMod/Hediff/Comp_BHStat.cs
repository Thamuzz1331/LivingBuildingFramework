using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class BHStatComp : BuildingHediffComp
    {
        public CompProperties_BuildingHediffStats Props => (CompProperties_BuildingHediffStats)props;

        public override void PostMake()
        {
            base.PostMake();
            parent.statMods.SetOrAdd(Props.stat, Props.mod);
        }
    }
}