using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace Verse
{
    public class BuildingGeneDef : Def
    {
        public Type buildingGeneClass = typeof(BuildingGene);
        public int architeCost = 0;
		public int complexity = 1;
		public int metabolicCost = 0;
		[Unsaved(false)]
		private Texture2D cachedIcon;
		[NoTranslate]
		public string iconPath;
		public Texture2D Icon
		{
			get
			{
				if (this.cachedIcon == null)
				{
					if (this.iconPath.NullOrEmpty())
					{
						this.cachedIcon = BaseContent.BadTex;
					}
					else
					{
						this.cachedIcon = (ContentFinder<Texture2D>.Get(this.iconPath, true) ?? BaseContent.BadTex);
					}
				}
				return this.cachedIcon;
			}
		}
		public Color IconColor
		{
			get
			{
				return Color.white;
			}
		}
		public string Description
        {
			get
            {
				return this.description;
            }
        }

		public static BuildingGeneDef Named(string defName)
        {
            return DefDatabase<BuildingGeneDef>.GetNamed(defName, true);
        }
		public List<string> props;
		public List<string> prerequisites;
    }
}