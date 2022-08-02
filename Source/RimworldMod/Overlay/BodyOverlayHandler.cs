using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace LivingBuilding
{
	// Token: 0x02000008 RID: 8
	[StaticConstructorOnStartup]
	internal class BodyOverlayHandler
	{
		public static HashSet<BuildingBody> bodies = new HashSet<BuildingBody>();

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
				foreach(BuildingBody body in bodies)
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
