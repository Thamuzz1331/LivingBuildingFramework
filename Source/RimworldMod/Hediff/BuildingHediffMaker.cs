using System;
using System.Collections.Generic;
using RimWorld;


namespace Verse
{
	public static class BuildingHediffMaker
	{
		public static BuildingHediff MakeBuildingHediff(BuildingHediffDef def)
		{
			BuildingHediff bhediff = (BuildingHediff)Activator.CreateInstance(def.buildingHediffClass);
			bhediff.def = def;
			bhediff.comps = new List<BuildingHediffComp>();
			bhediff.label = def.label;
			bhediff.visible = def.visible;
			foreach (CompProperties compProp in def.comps)
            {
				BuildingHediffComp c = (BuildingHediffComp)Activator.CreateInstance(compProp.compClass);
				c.props = compProp;
				c.parent = bhediff;
				bhediff.comps.Add(c);
            }
			bhediff.PostMake();
			return bhediff;
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