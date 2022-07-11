using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public interface IHeaddiff : IExposable
    {
        void Apply(CompBuildingBodyPart target);
        bool RunOnBodyParts();
        void Remove(CompBuildingBodyPart target);
    }
}