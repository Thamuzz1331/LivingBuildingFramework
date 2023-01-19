using System;

namespace LivingBuildings
{
	public static class BuildingGeneMaker
	{
		public static BuildingGene MakeBuildingGene(BuildingGeneDef def)
		{
			BuildingGene bgene = (BuildingGene)Activator.CreateInstance(def.buildingGeneClass);
			return bgene;
		}

/*		public static Hediff Debug_MakeConcreteExampleHediff(HediffDef def)
		{
			Hediff hediff = (Hediff)Activator.CreateInstance(def.hediffClass);
			hediff.def = def;
			hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
			hediff.PostMake();
			return hediff;
		}*/
	}
}