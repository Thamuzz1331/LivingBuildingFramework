using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace LivingBuilding
{
	[StaticConstructorOnStartup]
	internal static class Patching
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00002F2C File Offset: 0x0000112C
		static Patching()
		{
			Harmony harmony = new Harmony("LivingBuildingPatch");
			harmony.PatchAll();
		}
	}


	[HarmonyPatch(typeof(MapInterface), "MapInterfaceUpdate")]
	internal class MapInterfacePatch
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002DC4 File Offset: 0x00000FC4
		[HarmonyPostfix]
		public static void PostFix()
		{
			bool flag = BodyOverlayHandler.Instance.bodyOverlayToggle && Find.CurrentMap != null && !WorldRendererUtility.WorldRenderedNow;
			if (flag)
			{
				BodyOverlayHandler.Instance.UpdateBodyOverlay();
			}
		}
	}

	[HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
	internal class PlaySettingsPatch
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002D70 File Offset: 0x00000F70
		[HarmonyPostfix]
		public static void PostFix(WidgetRow row, bool worldView)
		{
			bool flag = !worldView && row != null;
			if (flag)
			{
				row.ToggleableIcon(ref BodyOverlayHandler.Instance.bodyOverlayToggle, BodyOverlayHandler.toggleButton, "BodyToggleTooltip".Translate(), SoundDefOf.Mouseover_ButtonToggle, null);
			}
		}
	}
}
