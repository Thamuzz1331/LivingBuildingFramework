using System;
using RimWorld;

namespace Verse
{
	public static class BuildingGeneMaker
	{
		public static BuildingGene MakeBuildingGene(BuildingGeneDef def, bool isGeneline = false)
		{
			BuildingGene bgene = (BuildingGene)Activator.CreateInstance(def.buildingGeneClass);
			bgene.def = def;
			bgene.label = def.label;
			bgene.geneLineGene = isGeneline;
			bgene.PostMake();
			bgene.loadID = Find.UniqueIDsManager.GetNextGeneID();
			return bgene;
		}
	}
}