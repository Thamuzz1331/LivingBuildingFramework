using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompScaffoldConverter : ThingComp
	{
		public CompProperties_ScaffoldConverter Props => (CompProperties_ScaffoldConverter)props;

		public List<Thing> toConvert = new List<Thing>();
		public HashSet<Thing> inToConvert = new HashSet<Thing>();

		public string bodyId = null;
		public BuildingBody body = null;

		private int age;

		public virtual float GetConversionWaitLength()
        {
			float cInterval = Props.conversionInterval;
			if (body != null && body.heart != null)
            {
				cInterval = cInterval / (body.heart.GetStat("growthSpeed"));
            }
			return cInterval;
        }
		public virtual float GetConversionCost()
        {
			float cost = Props.conversionCost;
			if (body != null && body.heart != null)
            {
				cost = cost / (body.heart.GetStat("growthEfficiency"));
            }
			return cost;
        }

		private float ticksToConversion;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref bodyId, "bodyId", "");
			Scribe_Values.Look(ref age, "age", 0);
			Scribe_Values.Look(ref ticksToConversion, "ticksToConversion", 0);
			Scribe_Collections.Look<Thing>(ref toConvert, "toConvert", LookMode.Reference);
			inToConvert.AddRange(toConvert);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (respawningAfterLoad)
            {
				((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
				foreach(Thing t in body.bodyParts)
                {
					foreach(Thing scaff in t.TryGetComp<CompBuildingBodyPart>().GetScaff())
                    {
						AddToConvert(scaff);
					}
					t.TryGetComp<CompBuildingBodyPart>().ClearScaff();
				}
			}
		}

		public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
			base.PostDeSpawn(map);
		}

		public override void CompTick()
		{
			if (!parent.Spawned || body == null)
			{
				return;
			}
			age++;
			ticksToConversion--;
			if (ticksToConversion <= 0)
			{
				ticksToConversion = GetConversionWaitLength();
				ConvertScaffold();
			}
		}

		public virtual void AddToConvert(Thing t)
        {
			if (!inToConvert.Contains(t) 
				&& t.TryGetComp<CompScaffold>() != null 
				&& !t.TryGetComp<CompScaffold>().transforming)
            {
				toConvert.Add(t);
				inToConvert.Add(t);
            }
		}

		public virtual void DetectionPulse()
		{
			List<Thing> initialScaffolds = new List<Thing>();
			foreach(IntVec3 c in GenAdj.CellsOccupiedBy(parent))
            {
				foreach (Thing adj in c.GetThingList(parent.Map))
				{
					if (adj.TryGetComp<CompScaffold>() != null &&
						parent.TryGetComp<CompBuildingBodyPart>() != null &&
						adj.TryGetComp<CompScaffold>().Props.species == parent.TryGetComp<CompBuildingBodyPart>().GetSpecies())
					{
						initialScaffolds.Add(adj);
					}
				}
			}
			foreach(Thing toReplace in initialScaffolds)
            {
				if (toReplace.TryGetComp<CompScaffold>() != null)
                {
					Thing replacement = toReplace.TryGetComp<CompScaffold>().Convert(this);
					if (replacement != null)
                    {
						RandEnqueue(replacement);
					}
				}
            }
		}

		public virtual void RandEnqueue(Thing t)
        {
			if (Rand.Chance(0.3f))
            {
				EnqueueSpur(t);
			}
			EnqueueSurrounding(t);
		}

		public virtual void EnqueueSurrounding(Thing t)
        {
			foreach(IntVec3 c in GenAdjFast.AdjacentCellsCardinal(t.Position).InRandomOrder())
            {
				foreach (Thing adj in c.GetThingList(parent.Map))
				{
					CompScaffold scaff = adj.TryGetComp<CompScaffold>();
					if (scaff != null && scaff.GetSpecies() == body.GetSpecies())
					{
						AddToConvert(adj);
					}
				}
			}
        }

		public virtual void EnqueueSpur(Thing t)
        {
			int numSpur = Rand.Range(1, 3);
			for (int i = 0; i < numSpur; i++)
            {
				int spurDepth = Rand.Range(3, 7);
				Vector3 vec = Rand.InsideUnitCircleVec3;
				bool cont = true;
				bool staple = false;
				for (int j = 2; j < spurDepth && cont && !staple; j++)
                {
					cont = false;
					IntVec3 c = t.Position + (vec * j).ToIntVec3();
					foreach (Thing adj in c.GetThingList(parent.Map))
					{
						if (adj.TryGetComp<CompScaffold>() != null && 
							parent.TryGetComp<CompBuildingBodyPart>() != null && 
							adj.TryGetComp<CompScaffold>().Props.species == parent.TryGetComp<CompBuildingBodyPart>().GetSpecies())
						{
							AddToConvert(adj);
							cont = true;
						}
					}
				}
			}
        }

		public virtual List<Thing> ConvertScaffold(bool instant = false, bool free = false)
		{
			List<Thing> ret = new List<Thing>();
			if (toConvert.Count <= 0)
            {
				return ret;
            }
			int numSpawn = GetNum();
			for (int i = 0; i < numSpawn; i++)
			{
				if (free || body.RequestNutrition(GetConversionCost())) {
					Thing toReplace = null;
					bool searching = true;
					while (searching)
					{
						if (toConvert.Count <= 0)
							return ret;
						toReplace = toConvert.First();
						toConvert.Remove(toReplace);
						inToConvert.Remove(toReplace);
						if (toReplace.TryGetComp<CompScaffold>() != null && !toReplace.Destroyed && !toReplace.TryGetComp<CompScaffold>().transforming)
						{
							searching = false;
						}
					}
					if (toReplace.TryGetComp<CompScaffold>() != null)
                    {
						Thing replacement = toReplace.TryGetComp<CompScaffold>().Convert(this, instant);
						if (replacement != null)
                        {
							ret.Add(replacement);
							RandEnqueue(replacement);
						}
					}
				}	
			}
			return ret;
		}

		//TODO: This is terrible, replace.
		public virtual int GetNum()
        {
			int ret = Rand.Range(1, 20);
			if (ret > 16)
            {
				ret = 3;
            } else if (ret > 12)
            {
				ret = 2;
            } else
            {
				ret = 1;
            }
			return ret;
        }

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			if (Prefs.DevMode)
	        {
				yield return new Command_Action
			    {
				    defaultLabel = "DEBUG: Grow All",
				    action = delegate()
				    {
						bool remaining = (toConvert.Count > 0);
						while (remaining)
                        {
							remaining = (0 != ConvertScaffold(true, true).Count);
                        }
				    }
			    };
			}
		}

		public virtual void CullToConvert()
        {
			Thing[] toCheck = new Thing[toConvert.Count];
			toConvert.CopyTo(toCheck);
			foreach(Thing c in toCheck)
            {
				bool remove = true;
				if (c == null)
				{
					toConvert.Remove(c);
                    inToConvert.Remove(c);
                }
                else
				{
					foreach (IntVec3 adj in GenAdj.CellsAdjacentCardinal(c))
					{
						foreach(Thing t in adj.GetThingList(c.Map))
						{
							if (t.TryGetComp<CompBuildingBodyPart>()?.bodyId == this.bodyId)
							{
								remove = false;
								break;
							}
						}
						if (remove)
						{
							toConvert.Remove(c);
							inToConvert.Remove(c);
						}
					}
                }
            }
        }

	}
	
}