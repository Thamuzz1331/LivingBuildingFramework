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
    public class BuildingGeneDef : Def
    {
        public Type buildingGeneClass = typeof(BuildingGene);
        public bool isArchoGene = false;
    }
}