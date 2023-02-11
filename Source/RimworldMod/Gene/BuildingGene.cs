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
	public class BuildingGene : IExposable, ILoadReferenceable
	{

		public BuildingGeneDef def;
		public string label;
		public bool geneLineGene = false;
		public bool Active = true;
		public int loadID;
		public bool Overridden
		{
			get
			{
				return this.overriddenByGene != null;
			}
		}
		public string GetUniqueLoadID()
		{
			return "BuildingGene_" + this.loadID;
		}
		public BuildingGene overriddenByGene = null;

		public virtual string LabelBase
		{
			get
			{
				return label;
			}
		}
		public virtual string LabelCap
		{
			get
			{
				return this.label.CapitalizeFirst();
			}
		}


		void IExposable.ExposeData()
		{
			Scribe_Defs.Look(ref def, "def");
			Scribe_Values.Look<string>(ref label, "label", "");
			Scribe_Values.Look<bool>(ref geneLineGene, "geneLineGene", false);
			Scribe_Values.Look<int>(ref loadID, "loadID", 0);
			Scribe_References.Look<BuildingGene>(ref this.overriddenByGene, "overriddenByGene", false);
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
			foreach(BuildingGene g in core.genes)
            {
				if (this.OverridesGene(g))
                {
					g.overriddenByGene = this;
					g.PostRemove(core);
                }
            }
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
		public virtual bool OverridesGene(BuildingGene b)
        {
			return false;
        }
	}
}