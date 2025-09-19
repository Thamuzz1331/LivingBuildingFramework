using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace LivingBuildings
{
	[StaticConstructorOnStartup]
	internal static class Patching
	{
		static Patching()
		{
			Harmony harmony = new Harmony("LivingBuildingPatch");
			harmony.PatchAll();
		}
	}


	[HarmonyPatch(typeof(MapInterface), "MapInterfaceUpdate")]
	internal class MapInterfacePatch
	{
		[HarmonyPostfix]
		public static void PostFix()
		{
			bool flag = BodyOverlayHandler.Instance.bodyOverlayToggle && Find.CurrentMap != null && !WorldRendererUtility.WorldRendered;
			if (flag)
			{
				BodyOverlayHandler.Instance.UpdateBodyOverlay();
			}
		}
	}

	[HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
	internal class PlaySettingsPatch
	{
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
