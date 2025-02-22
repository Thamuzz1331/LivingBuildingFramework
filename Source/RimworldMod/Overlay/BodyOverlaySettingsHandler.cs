using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SettingsHelper;
using UnityEngine;
using Verse;

namespace RimWorld
{
	internal class BodyOverlaySettingsHandler : Mod
	{
		public BodyOverlaySettingsHandler(ModContentPack content) : base(content)
		{
			BodyOverlaySettingsHandler.Settings = base.GetSettings<BodyOverlaySettings>();
		}

		public override string SettingsCategory()
		{
			return "Living Building";
		}

		public override void DoSettingsWindowContents(Rect rect)
		{
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(rect);
			listing_Standard.Gap(5f);
			listing_Standard.Label("BodyOpacitySetting".Translate(), -1f, null);
			BodyOverlaySettingsHandler.Settings.opacity = listing_Standard.Slider(BodyOverlaySettingsHandler.Settings.opacity, 0f, 1f);
			listing_Standard.Gap(5f);
			listing_Standard.AddLabeledNumericalTextField("BodyOverlayUpdateTicks", ref BodyOverlaySettingsHandler.Settings.baseUpdateTicks, 0.5f, 1f, 100000f);
			if (listing_Standard.ButtonText("ResetBodySettings".Translate(), null))
			{
				this.ResetAll();
			}
			listing_Standard.End();
			base.DoSettingsWindowContents(rect);
		}

		private void ResetAll()
		{
			BodyOverlaySettingsHandler.Settings.opacity = 0.5f;
			BodyOverlaySettingsHandler.Settings.baseUpdateTicks = 120;
		}

		public static BodyOverlaySettings Settings;
	}
}
