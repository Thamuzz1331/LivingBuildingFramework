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
		protected CompProperties_ScaffoldConverter Props => (CompProperties_ScaffoldConverter)props;

		public Queue<Thing> toConvert = new Queue<Thing>();

		public string bodyId = null;
		private BuildingBody body = null;

		private int age;

		public float conversionWaitLength = 45f;
		public virtual float GetConversionWaitLength()
        {
			return conversionWaitLength;
        }

		private float ticksToConversion;

		public float AgeDays => (float)age / 60000f;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref bodyId, "bodyId", "");
			Scribe_Values.Look(ref age, "age", 0);
			Scribe_Values.Look(ref ticksToConversion, "ticksToConversion", 0);
			List<Thing> conversionList = toConvert.ToList<Thing>();
			Scribe_Collections.Look<Thing>(ref conversionList, "conversionList", LookMode.Deep);
			toConvert = new Queue<Thing>(conversionList);
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostPostMake();
			if (respawningAfterLoad)
            {
				((MapCompBuildingTracker)parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(parent.TryGetComp<CompScaffoldConverter>());
			}
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
				ticksToConversion = conversionWaitLength;
				ConvertHullTile();
			}
		}

		public virtual void DetectionPulse()
		{
			int startSpots = Rand.Range(4, 6);
			for (int s = 0; s < startSpots; s++)
            {
				IntVec3 c = parent.Position + (Rand.InsideUnitCircleVec3 * 3).ToIntVec3();
				foreach (Thing t in c.GetThingList(parent.Map))
				{
					if (t.TryGetComp<CompScaffold>() != null)
					{
						toConvert.Enqueue(t);
						EnqueueSpur(t);
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
						if (adj.TryGetComp<CompScaffold>() != null && adj.TryGetComp<CompScaffold>().Props.species == body.GetSpecies())
						{
							toConvert.Enqueue(adj);
							cont = true;
						}
					}
				}
			}
        }

		public virtual void ConvertHullTile()
		{
			if (toConvert.Count <= 0)
            {
				return;
            }
			int numSpawn = GetNum();
			for (int i = 0; i < numSpawn; i++)
			{
				if (body.RequestNutrition(body.GetConversionNutritionCost())) { 
					Thing toReplace = null;
					bool searching = true;
					while (searching)
					{
						if (toConvert.Count <= 0)
							return;
						toReplace = toConvert.Dequeue();
						if (toReplace.TryGetComp<CompScaffold>() != null && !toReplace.Destroyed)
						{
							searching = false;
						}
					}
					if(toReplace.TryGetComp<CompScaffold>() != null)
                    {
						Thing replacement = toReplace.TryGetComp<CompScaffold>().Convert(this);
						if (replacement != null)
                        {
							RandEnqueue(replacement);
						}
					}
				}	
			}

		}

		//This is terrible, replace.
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

	}
	
}