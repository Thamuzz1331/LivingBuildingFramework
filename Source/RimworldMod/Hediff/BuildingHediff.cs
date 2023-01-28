using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class BuildingHediff : IExposable
    {
		public BuildingHediffDef def;
		public bool visible = true;
		public string label = "";
		public Thing bodyPart;
		public CompBuildingBodyPart bp;
		public Dictionary<string, float> statMods = new Dictionary<string, float>();
		public List<BuildingHediffComp> comps;

		public virtual string LabelBase
		{
			get
			{
				return label;
			}
		}

		public virtual string DisplayLabel
        {
            get
            {
				return LabelBase;
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
			Scribe_Defs.Look(ref def, "def");
			Scribe_Values.Look<bool>(ref visible, "visible", true);
			Scribe_Values.Look<string>(ref label, "label", "");
			Scribe_References.Look<Thing>(ref bodyPart, "bodyPart");
			bp = bodyPart.TryGetComp<CompBuildingBodyPart>();
			Scribe_Collections.Look(ref statMods, "statMods");
			Scribe_Collections.Look(ref comps, "comps");
			PostExposeData();
		}

		public virtual void PostMake()
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.PostMake();
            }

        }

		public virtual void PostSpawnSetup(bool respawningAfterLoad)
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.PostSpawnSetup(respawningAfterLoad);
            }
        }

		public virtual void PostExposeData()
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.PostExposeData();
            }
        }

		public virtual void PostAdd()
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.PostAdd();
            }
			
        }

		public virtual void PostRemove()
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.PostRemove();
            }
        }

		public virtual void Tick()
        {
			foreach(BuildingHediffComp comp in comps)
            {
				comp.Tick();
            }
        }

		public virtual float StatMod(string stat)
        {
			return statMods.TryGetValue(stat, 1f);
        }
	}
}