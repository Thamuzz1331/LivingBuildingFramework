using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
    public class CompNutrition : ThingComp
    {
        public String bodyId = "NA";
        public BuildingBody body = null;

        public void SetId(String _id)
        {
            bodyId = _id;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            ((MapCompBuildingTracker)this.parent.Map.components.Where(t => t is MapCompBuildingTracker).FirstOrDefault()).Register(this);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<String>(ref bodyId, "bodyId", "NA");
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            if (body != null)
                body.DeRegister(this);
        }

        public override string CompInspectStringExtra()
        {
            if (body != null)
            {
                return "Nutrition " + body.nutritionGen + "/" + body.passiveConsumption + "\n" + 
                    body.currentNutrition + "/" + body.nutritionCapacity;
            }
            return "";
        }

    }
}