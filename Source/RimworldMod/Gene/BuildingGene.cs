using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace LivingBuildings
{
	public class BuildingGene : IExposable
	{
		public string label = "";
		public Thing bodyPart;
		public CompBuildingBodyPart bp;


		public virtual string LabelBase
		{
			get
			{
				return label;
			}
		}

		public virtual CompBuildingBodyPart BodyPart
		{
			get
			{
				return bp;
			}
		}

		void IExposable.ExposeData()
		{
			Scribe_Values.Look<string>(ref label, "label", "");
			Scribe_References.Look<Thing>(ref bodyPart, "bodyPart");
			bp = bodyPart.TryGetComp<CompBuildingBodyPart>();
			PostExposeData();
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