using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace LivingBuildings
{
    public class BuildingHediffDef : Def
    {
        public Type buildingHediffClass = typeof(Hediff_Building);
        public float initialSeverity = 0;
        public bool isBad = false;
        [MustTranslate]
        public string labelNoun;
        [MustTranslate]
        public string labelNounPretty;
    }
}