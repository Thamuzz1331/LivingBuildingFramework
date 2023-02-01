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
		public Thing bodyPart;
		public CompBuildingBodyPart bp;
		public Dictionary<string, float> statMods = new Dictionary<string, float>();
		public List<BuildingHediffComp> comps;

		public virtual string LabelBase
		{
			get
			{
				return def.LabelCap;
			}
		}

		public virtual string DisplayLabel
        {
            get
            {
				return LabelBase;
            }
        }

		public virtual Color DisplayColor
        {
			get
            {
				return def.displayColor;
            }
        }

		public virtual string HoverText
        {
			get
            {
				string ret = def.description + "\n";
				foreach(string stat in statMods.Keys)
                {
					ret += "\n" + stat.Translate() + ": x" + this.StatMod(stat);
                }
				return ret;
            }
        }

		public virtual bool Visible
        {
            get
            {
				return def.visible;
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