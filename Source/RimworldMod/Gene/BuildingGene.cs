using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI.Group;

namespace Verse
{
	public class BuildingGene : IExposable
	{
		public BuildingGeneDef def;
		public string label;
		public bool geneLineGene = false;
		public virtual string LabelBase
		{
			get
			{
				return label;
			}
		}

		void IExposable.ExposeData()
		{
			Scribe_Defs.Look(ref def, "def");
			Scribe_Values.Look<string>(ref label, "label", "");
			Scribe_Values.Look<bool>(ref geneLineGene, "geneLineGene", false);
			PostExposeData();
		}

		public virtual void PostSpawnSetup(bool respawningAfterLoad)
		{

		}

		public virtual void PostExposeData()
		{

		}

		public virtual void PostAdd(CompBuildingCore core)
		{
		}

		public virtual void PostRemove(CompBuildingCore core)
        {
        }

		public virtual void PostMake()
        {

        }

		public virtual void Tick()
		{

		}
	}
}