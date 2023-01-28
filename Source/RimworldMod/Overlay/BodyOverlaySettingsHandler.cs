using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SettingsHelper;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000002 RID: 2
	internal class BodyOverlaySettingsHandler : Mod
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public BodyOverlaySettingsHandler(ModContentPack content) : base(content)
		{
			BodyOverlaySettingsHandler.Settings = base.GetSettings<BodyOverlaySettings>();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		public override string SettingsCategory()
		{
			return "Living Building";
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
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

		// Token: 0x06000004 RID: 4 RVA: 0x000022BC File Offset: 0x000004BC
		private void ResetAll()
		{
			BodyOverlaySettingsHandler.Settings.opacity = 0.5f;
			BodyOverlaySettingsHandler.Settings.baseUpdateTicks = 120;
		}

		// Token: 0x04000001 RID: 1
		public static BodyOverlaySettings Settings;
	}
}
