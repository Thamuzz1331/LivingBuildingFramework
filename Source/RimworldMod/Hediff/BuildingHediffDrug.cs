using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
	public class BuildingHediff_Drug : BuildingHediff
	{
		public float durationTicks = 0;
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref durationTicks, "durationTicks", 0);
		}

		public override void Tick()
        {
			base.Tick();
			if (durationTicks <= 0)
            {
				bp.RemoveHediff(this);
            }
			durationTicks--;
        }
	}
}