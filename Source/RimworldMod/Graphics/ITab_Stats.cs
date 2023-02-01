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
			this.size = new Vector2(630f, 430f);
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
			Rect statRect = new Rect(0, 0, this.size.x * 0.375f, this.size.y).Rounded();
			DrawStats(statRect);
			DrawHediffs(new Rect(statRect.xMax, 0, this.size.x-statRect.width, this.size.y).ContractedBy(10f));
		}

		protected void DrawStats(Rect statsRect)
        {
			GUI.color = Color.white;
			Widgets.DrawMenuSection(statsRect);
			statsRect = statsRect.ContractedBy(9f);
			Widgets.BeginGroup(statsRect);
			float curY = 0f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = new Color(0.9f, 0.9f, 0.9f);
			Rect nameRect = new Rect(0f, curY, statsRect.width, 34f);
			Widgets.Label(nameRect, core.bodyName);

			Text.Font = GameFont.Small;
			curY += 34f;

			foreach(string stat in core.stats.Keys)
            {

				Rect statRect = new Rect(0f, curY, statsRect.width, 20f);
				if (Mouse.IsOver(statRect))
				{
					GUI.color = ITab_Stats.HighlightColor;
					GUI.DrawTexture(statRect, TexUI.HighlightTex);
				}
				GUI.color = Color.white;
				Widgets.Label(new Rect(0f, curY, statsRect.width * 0.65f, 30f), stat.Translate());
				
				Pair<string, Color> efficiencyLabel = ITab_Stats.GetEfficiencyLabel(core.GetStat(stat));
				GUI.color = efficiencyLabel.Second;
				Widgets.Label(new Rect(statsRect.width * 0.65f, curY, statsRect.width * 0.35f, 30f), efficiencyLabel.First);
				curY += 20f;
			}
			Widgets.EndGroup();
        }

		protected void DrawHediffs(Rect hediffRect)
        {
			GUI.color = Color.white;
			if (Prefs.DevMode && Current.ProgramState == ProgramState.Playing)
			{
				this.DoDebugOptions(hediffRect);
			}
			Widgets.BeginGroup(hediffRect);
			float lineHeight = Text.LineHeight;
			Rect outRect = new Rect(0f, 0f, hediffRect.width, hediffRect.height - lineHeight);
			Rect viewRect = new Rect(0f, 0f, hediffRect.width - 16f, ITab_Stats.scrollViewHeight);
			Rect rect2 = hediffRect;
			if (viewRect.height > outRect.height)
			{
				rect2.width -= 16f;
			}
			Widgets.BeginScrollView(outRect, ref ITab_Stats.scrollPosition, viewRect, true);
			GUI.color = Color.white;
			float curY = 0f;
			curY = DrawBPHediffs(rect2, curY, core, core.hediffs);
			Dictionary<CompBuildingBodyPart, List<BuildingHediff>> bpToHediff = GetBodyPartsToHediffs(core);
			foreach (CompBuildingBodyPart bp in bpToHediff.Keys)
            {
				curY = DrawBPHediffs(rect2, curY, bp, bpToHediff.TryGetValue(bp, null));
            }
			Widgets.EndScrollView();
			Widgets.EndGroup();
        }

		public float DrawBPHediffs(Rect rect, float curY, CompBuildingBodyPart bp, List<BuildingHediff> hediffs)
        {
			if (hediffs.Count == 0)
            {
				return curY;
            }
			float bpLabelWidth = rect.width * 0.375f;
			float diffLabelWidth = rect.width - bpLabelWidth;
			float textHeight = Text.CalcHeight(bp.parent.def.LabelCap, bpLabelWidth);
			float rowsUsed = hediffs.Count;
			Rect highlightRect = new Rect(rect.x, curY, rect.width, textHeight * rowsUsed);
			ITab_Stats.DoRightRowHighlight(highlightRect);
			Rect bpLabel = new Rect(0, curY, bpLabelWidth, textHeight);
			Widgets.Label(bpLabel, bp.parent.def.LabelCap);
			foreach(BuildingHediff diff in hediffs)
            {
				float th = Text.CalcHeight(diff.DisplayLabel, diffLabelWidth);
				Rect diffLabel = new Rect(bpLabelWidth, curY, diffLabelWidth, th);
				Widgets.DrawHighlightIfMouseover(diffLabel);
				GUI.color = diff.DisplayColor;
				Widgets.Label(diffLabel, diff.DisplayLabel);
				GUI.color = Color.white;
				TooltipHandler.TipRegion(diffLabel, diff.HoverText);
				curY += th;
            }
			return curY;
        }

		public static Dictionary<CompBuildingBodyPart, List<BuildingHediff>> GetBodyPartsToHediffs(CompBuildingCore core)
        {
			Dictionary<CompBuildingBodyPart, List<BuildingHediff>> ret = new Dictionary<CompBuildingBodyPart, List<BuildingHediff>>();
			foreach(BuildingHediff hediff in core.hediffs)
            {
				if (hediff.bp != core)
                {
					if (!ret.ContainsKey(hediff.bp))
					{
						ret.Add(hediff.bp, new List<BuildingHediff>());
					}
					ret.TryGetValue(hediff.bp, null).Add(hediff);
				}
            }
			return ret;
        }

		public void DoDebugOptions(Rect hediffRect) {
		}

		public static Pair<string, Color> GetEfficiencyLabel(float stat)
		{
			return new Pair<string, Color>(stat.ToStringPercent(), ITab_Stats.efficiencyToColor[HealthCardUtility.EfficiencyValueToEstimate(stat)]);
		}
		private static float scrollViewHeight = 0f;
		private static Vector2 scrollPosition = Vector2.zero;

		private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private static readonly Color StaticHighlightColor = new Color(0.75f, 0.75f, 0.85f, 1f);
		private static bool highlight = true;

		private static void DoRightRowHighlight(Rect rowRect)
		{
			if (ITab_Stats.highlight)
			{
				GUI.color = ITab_Stats.StaticHighlightColor;
				GUI.DrawTexture(rowRect, TexUI.HighlightTex);
			}
			ITab_Stats.highlight = !ITab_Stats.highlight;
		}


		private static readonly Dictionary<EfficiencyEstimate, Color> efficiencyToColor = new Dictionary<EfficiencyEstimate, Color>
		{
			{
				EfficiencyEstimate.None,
				ColorLibrary.RedReadable
			},
			{
				EfficiencyEstimate.VeryPoor,
				new Color(0.75f, 0.45f, 0.45f)
			},
			{
				EfficiencyEstimate.Poor,
				new Color(0.55f, 0.55f, 0.55f)
			},
			{
				EfficiencyEstimate.Weakened,
				new Color(0.7f, 0.7f, 0.7f)
			},
			{
				EfficiencyEstimate.GoodCondition,
				HealthUtility.GoodConditionColor
			},
			{
				EfficiencyEstimate.Enhanced,
				new Color(0.5f, 0.5f, 0.9f)
			}
		};
	}
}