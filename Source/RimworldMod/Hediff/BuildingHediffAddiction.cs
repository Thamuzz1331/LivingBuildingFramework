using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
	public class Building_Addiction : BuildingHediff
	{
		public float withdrawl = 0f;
		public float maxWithdrawl = 0f;
		public float withdrawRate = 0f;
		public float massMult = 0f;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref withdrawl, "withdrawl", 0);
			Scribe_Values.Look<float>(ref maxWithdrawl, "maxWithdrawl", 0);
			Scribe_Values.Look<float>(ref withdrawRate, "withdrawRate", 0);
			Scribe_Values.Look<float>(ref massMult, "massMult", 0);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (respawningAfterLoad)
            {
				bp.body.addictions.Add(this);
			}
		}

		public override void PostAdd()
        {
			base.PostAdd();
			bp.body.addictions.Add(this);
        }

		public override void PostRemove()
        {
			base.PostRemove();
        }

		public override float StatMod(string stat)
		{
			float mod = statMods.TryGetValue(stat, 1f) - 1;
			mod = (withdrawl / maxWithdrawl) * mod;
			return 1+mod;
		}

		public override void Tick()
        {
			base.Tick();
			if (withdrawl >= maxWithdrawl)
            {
				bp.body.addictions.Remove(this);
				bp.RemoveHediff(this);
			}
        }
	}
}