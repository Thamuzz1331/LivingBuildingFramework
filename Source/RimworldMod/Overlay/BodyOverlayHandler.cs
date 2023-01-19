using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace LivingBuildings
{
	// Token: 0x02000008 RID: 8
	[StaticConstructorOnStartup]
	internal class BodyOverlayHandler
	{
		public static HashSet<MapCompBuildingTracker> bodiesHandlers = new HashSet<MapCompBuildingTracker>();

		public static BodyOverlayHandler Instance
		{
			get
			{
				if (BodyOverlayHandler.current == null)
				{
					BodyOverlayHandler.current = new BodyOverlayHandler();
				}
				return BodyOverlayHandler.current;
			}
		}

		internal void UpdateBodyOverlay()
		{
			if (this.bodyOverlayToggle)
			{
				Map curMap = Find.CurrentMap;
				MapCompBuildingTracker curTracker = null;
				foreach(MapCompBuildingTracker tracker in bodiesHandlers)
                {
					if (tracker.map == curMap)
                    {
						curTracker = tracker;
                    }
                }
				if (curTracker == null)
                {
					return;
                }
				foreach(BuildingBody body in curTracker.bodies.Values)
                {
					body.Drawer.CellBoolDrawerUpdate();
				}
			}
		}

		public bool bodyOverlayToggle;

		public static readonly Texture2D toggleButton = ContentFinder<Texture2D>.Get("ToggleButton", true);

		private static BodyOverlayHandler current;
	}
}
