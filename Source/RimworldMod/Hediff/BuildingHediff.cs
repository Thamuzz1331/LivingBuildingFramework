using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
    public class Hediff_Building : IExposable
    {
		public string label = "";
		public bool visible = true;
		public Thing bodyPart;
		public CompBuildingBodyPart bp;
		public Dictionary<string, float> statMods = new Dictionary<string, float>();

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
			Scribe_Values.Look<bool>(ref visible, "visible", true);
			Scribe_References.Look<Thing>(ref bodyPart, "bodyPart");
			bp = bodyPart.TryGetComp<CompBuildingBodyPart>();
			Scribe_Collections.Look(ref statMods, "statMods");
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

		public virtual void Tick()
        {

        }

		public virtual float StatMod(string stat)
        {
			return statMods.TryGetValue(stat, 1f);
        }
	}
}