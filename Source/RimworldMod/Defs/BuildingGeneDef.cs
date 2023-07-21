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
		public bool RandomChosen { get; set; }

        public bool ConflictsWith(BuildingGeneDef other)
        {
            if (this == other)
            {
                return true;
            }
            if (this.geneOverrides != null && other.geneOverrides != null)
            {
                for (int i = 0; i < this.geneOverrides.Count; i++)
                {
                    if (other.geneOverrides.Contains(this.geneOverrides[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

		public bool Overrides(BuildingGeneDef other, bool isXenogene, bool otherIsXenogene)
        {
			if (this.RandomChosen || other.RandomChosen || !this.ConflictsWith(other))
			{
				return false;
			}
			if (isXenogene == otherIsXenogene)
			{
				return BuildingGeneUtils.GenesInOrder.IndexOf(this) <= BuildingGeneUtils.GenesInOrder.IndexOf(other);
			}
			return isXenogene && !otherIsXenogene;
        }

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

		public string DescriptionFull
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
        public Type buildingGeneClass = typeof(BuildingGene);
		public GeneCategoryDef displayCategory;
		public float displayOrderInCategory;
		public bool showGizmoOnMultiSelect;
		public List<AbilityDef> abilities;
		public List<string> props;
		public List<string> prerequisites;
		public bool randomChosen;
        public int architeCost = 0;
		public int complexity = 1;
		public int metabolicCost = 0;
		public BuildingGeneDef prerequisite;
		[Unsaved(false)]
		private Texture2D cachedIcon;
		[NoTranslate]
		public string iconPath;
		public List<string> tags = new List<string>();
		public List<string> geneOverrides = new List<string>();

    }

	public static class BuildingGeneUtils
    {
		public static List<BuildingGeneDef> NonOverriddenGenes(this List<BuildingGeneDef> geneDefs, bool xenogene)
		{
			BuildingGeneUtils.tmpGeneDefsWithType.Clear();
			foreach (BuildingGeneDef geneDef in geneDefs)
			{
				BuildingGeneUtils.tmpGeneDefsWithType.Add(new BuildingGeneDefWithType(geneDef, xenogene));
			}
			return BuildingGeneUtils.tmpGeneDefsWithType.NonOverriddenGenes();
		}

		public static List<BuildingGeneDef> NonOverriddenGenes(this List<BuildingGeneDefWithType> geneDefWithTypes)
		{
			BuildingGeneUtils.tmpGenes.Clear();
			BuildingGeneUtils.tmpOverriddenGenes.Clear();
			if (!ModsConfig.BiotechActive)
			{
				return BuildingGeneUtils.tmpGenes;
			}
			foreach (BuildingGeneDefWithType geneDefWithType in geneDefWithTypes)
			{
				BuildingGeneUtils.tmpGenes.Add(geneDefWithType.geneDef);
			}
			for (int i = 0; i < geneDefWithTypes.Count; i++)
			{
				if (!geneDefWithTypes[i].RandomChosen)
				{
					for (int j = i + 1; j < geneDefWithTypes.Count; j++)
					{
						if (!geneDefWithTypes[j].RandomChosen && geneDefWithTypes[i].ConflictsWith(geneDefWithTypes[j]))
						{
							if (geneDefWithTypes[i].Overrides(geneDefWithTypes[j]))
							{
								BuildingGeneUtils.tmpOverriddenGenes.Add(geneDefWithTypes[j]);
							}
							else
							{
								BuildingGeneUtils.tmpOverriddenGenes.Add(geneDefWithTypes[i]);
							}
						}
					}
				}
			}
			foreach (BuildingGeneDefWithType geneDefWithType2 in BuildingGeneUtils.tmpOverriddenGenes)
			{
				BuildingGeneUtils.tmpGenes.Remove(geneDefWithType2.geneDef);
			}
			BuildingGeneUtils.tmpOverriddenGenes.Clear();
			return BuildingGeneUtils.tmpGenes;
		}

		public static List<BuildingGeneDef> GenesInOrder
		{
			get
			{
				if (BuildingGeneUtils.cachedGeneDefsInOrder == null)
				{
					BuildingGeneUtils.cachedGeneDefsInOrder = new List<BuildingGeneDef>();
					foreach (BuildingGeneDef geneDef in DefDatabase<BuildingGeneDef>.AllDefs)
					{
						BuildingGeneUtils.cachedGeneDefsInOrder.Add(geneDef);
					}
					BuildingGeneUtils.cachedGeneDefsInOrder.SortBy((BuildingGeneDef x) => -x.displayCategory.displayPriorityInXenotype, (BuildingGeneDef x) => x.displayCategory.label, (BuildingGeneDef x) => x.displayOrderInCategory);
				}
				return BuildingGeneUtils.cachedGeneDefsInOrder;
			}
		}

        private static List<BuildingGeneDef> cachedGeneDefsInOrder = null;
		private static List<BuildingGeneDef> tmpGenes = new List<BuildingGeneDef>();
		private static HashSet<BuildingGeneDefWithType> tmpOverriddenGenes = new HashSet<BuildingGeneDefWithType>();
		private static List<BuildingGeneDefWithType> tmpGeneDefsWithType = new List<BuildingGeneDefWithType>();
    }


}