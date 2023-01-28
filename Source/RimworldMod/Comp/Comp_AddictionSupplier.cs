using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class CompAddictionSupplier : ThingComp
    {
        public CompProperties_AddictionSupplier Props => (CompProperties_AddictionSupplier)props;

        public virtual bool CanSupply()
        {
            return false;
        }

        public virtual void SetConsumption(float consumptionRate)
        {

        }

    }
}