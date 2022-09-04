using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class ITab_Stats : ITab
	{
//		private Vector2 scrollPos;

		public ITab_Stats()
		{
			this.labelKey = "LivingBuildings.TabStats";
			this.size = new Vector2(600f, 520f);
		}

		public override bool IsVisible
		{
			get
			{
				Thing sel = base.SelThing;
				CompBuildingCore core = sel.TryGetComp<CompBuildingCore>();
				return (core != null);
			}
		}
		private CompBuildingCore core;
		protected override void FillTab()
		{
			Thing sel = base.SelThing;
			core = sel.TryGetComp<CompBuildingCore>();
			Rect rect = new Rect(20f, 30f, this.size.x/2, 30f);
			foreach(string stat in core.stats.Keys)
            {
				Widgets.Label(rect, stat.Translate());
				rect = new Rect(this.size.x/2, rect.y, this.size.x/2, 30f);
				Widgets.Label(rect, (core.GetStat(stat)*100 + "%"));
				rect = new Rect(20f, rect.y + 30f, this.size.x / 2, 30f);
			}
		}

	}
}