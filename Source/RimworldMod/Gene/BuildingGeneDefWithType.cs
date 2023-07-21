using System;
using Verse;

namespace RimWorld
{

	public struct BuildingGeneDefWithType
	{

		public BuildingGeneDefWithType(BuildingGeneDef _geneDef, bool _xenogene)
		{
			this.geneDef = _geneDef;
			this.isXenogene = _xenogene;
		}

		public bool RandomChosen
		{
			get
			{
				return this.geneDef.RandomChosen;
			}
		}

		public bool ConflictsWith(BuildingGeneDefWithType other)
		{
			return this.geneDef.ConflictsWith(other.geneDef);
		}

		public bool Overrides(BuildingGeneDefWithType other)
		{
			return this.geneDef.Overrides(other.geneDef, this.isXenogene, other.isXenogene);
		}

		public BuildingGeneDef geneDef;

		public bool isXenogene;
	}
}