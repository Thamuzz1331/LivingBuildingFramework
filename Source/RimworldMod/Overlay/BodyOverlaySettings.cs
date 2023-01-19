using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace LivingBuildings
{
	// Token: 0x02000003 RID: 3
	internal class BodyOverlaySettings : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.baseUpdateTicks, "OverlayUpdateTicks", 0, false);
			Scribe_Values.Look<float>(ref this.opacity, "OverlayOpacity", 0f, false);
			base.ExposeData();
		}

		// Token: 0x04000002 RID: 2
		public float opacity = 0.5f;

		// Token: 0x04000003 RID: 3
		public int baseUpdateTicks = 120;
	}
}
