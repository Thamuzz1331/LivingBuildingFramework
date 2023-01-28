using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace Verse
{
    public class BuildingHediffDef : Def
    {
        public Type buildingHediffClass = typeof(BuildingHediff);
        public bool isBad = false;
        public bool visible = true;
//        [MustTranslate]
//        public string labelNoun;
//        [MustTranslate]
//        public string labelNounPretty;
        public List<CompProperties> comps;

        public static BuildingHediffDef Named(string defName)
        {
            return DefDatabase<BuildingHediffDef>.GetNamed(defName, true);
        }

    }
}