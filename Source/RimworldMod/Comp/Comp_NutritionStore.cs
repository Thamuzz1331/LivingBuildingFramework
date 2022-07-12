using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompNutritionStore : CompNutrition
    {
        public CompProperties_NutritionStore Props => (CompProperties_NutritionStore)props;
        public float currentNutrition = 0f;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!respawningAfterLoad)
            {
                this.currentNutrition = Props.initialNutrition;
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public virtual float getNutrientCapacity()
        {
            return Props.nutrientCapacity;
        }
        public virtual float getCurrentNutrition()
        {
            return currentNutrition;
        }
        public virtual float storeNutrition(float qty)
        {
            float overflow = 0f;
            float toStore = qty;
            currentNutrition += toStore;
            overflow = currentNutrition - getNutrientCapacity();
            if (overflow <= 0)
            {
                return 0;
            }
            currentNutrition = getNutrientCapacity();
            return overflow;
        }
        public virtual float consumeNutrition(float qty)
        {
            currentNutrition -= qty;
            if (currentNutrition < 0)
            {
                float ret = currentNutrition;
                currentNutrition = 0;
                return ret * -1;
            }
            return 0;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref currentNutrition, "currentNutrition", 0f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
	        foreach (Gizmo gizmo in base.CompGetGizmosExtra())
	        {
		        yield return gizmo;
	        }
	        if (Prefs.DevMode)
	        {
		        if ((this.getNutrientCapacity() - this.getCurrentNutrition()) > 0f)
		        {
			        yield return new Command_Action
			        {
				        defaultLabel = "DEBUG: Fill",
				        action = delegate()
				        {
					        this.currentNutrition = this.getNutrientCapacity();
				        }
			        };
		        }
		        if (this.currentNutrition > 0f)
		        {
			        yield return new Command_Action
			        {
				        defaultLabel = "DEBUG: Empty",
				        action = delegate()
				        {
                            this.currentNutrition = 0f;
				        }
			        };
		        }
	        }
        }
    }
}