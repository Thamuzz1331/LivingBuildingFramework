using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class BuildingHediffComp : IExposable
    {
        public BuildingHediff parent;
        public CompProperties props;

		void IExposable.ExposeData()
        {
			PostExposeData();
		}

        public virtual void PostMake()
        {

        }

		public virtual void PostSpawnSetup(bool respawningAfterLoad)
        {

        }

		public virtual void PostExposeData()
        {

        }

		public virtual void PostAdd()
        {
			
        }

		public virtual void PostRemove()
        {

        }

		public virtual void Tick()
        {

        }
	}
}