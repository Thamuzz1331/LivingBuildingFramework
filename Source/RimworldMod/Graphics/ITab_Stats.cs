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
			Rect rect = new Rect(10f, 30f, this.size.x/4, 30f);
			foreach(string stat in core.stats.Keys)
            {
				Widgets.Label(rect, stat.Translate());
				rect = new Rect(this.size.x/4, rect.y, this.size.x/4, 30f);
				string statVal = (Math.Truncate(core.GetStat(stat)*10000)/100) + "%";
				Widgets.Label(rect, statVal);
				rect = new Rect(20f, rect.y + 30f, this.size.x/4, 30f);
			}

			rect = new Rect(this.size.x/2, 30f, this.size.x/2, 30f);
			Widgets.Label(rect, core.ToString());
			foreach(BuildingHediff diff in core.hediffs)
            {
				Log.Message("Diff visible " + diff.visible);
				if (diff.visible)
                {
					rect = new Rect(this.size.x/2+20f, rect.y+30, this.size.x/2-20f, 30f);
					Widgets.Label(rect, diff.LabelBase);
                }
            }
			rect = new Rect(this.size.x/2, rect.y+30, this.size.x/2, 30f);
			foreach(Thing b in core.body.bodyParts)
            {
				CompBuildingBodyPart bp = b.TryGetComp<CompBuildingBodyPart>();
				if (bp.VisibleHediffs)
                {
					Widgets.Label(rect, bp.ToString());
					foreach(BuildingHediff diff in bp.hediffs)
                    {
						if (diff.visible)
                        {
							rect = new Rect(this.size.x/2+20f, rect.y+30, this.size.x/2-20f, 30f);
							Widgets.Label(rect, diff.LabelBase);
                        }
                    }
					rect = new Rect(this.size.x/2, rect.y+30, this.size.x/2, 30f);
                }
            }
		}

	}
}