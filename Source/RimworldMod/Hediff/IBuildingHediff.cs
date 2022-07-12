using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public interface IHediff : IExposable
    {
        bool ShouldAddTo(CompBuildingBodyPart target);
        void Apply(CompBuildingBodyPart target);
        void Remove(CompBuildingBodyPart target);
    }
}