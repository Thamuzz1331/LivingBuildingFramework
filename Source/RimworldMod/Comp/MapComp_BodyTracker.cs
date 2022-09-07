using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;
using LivingBuilding;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class MapCompBuildingTracker : MapComponent
    {
        int curTick = 0;
        public Dictionary<String, BuildingBody> bodies = new Dictionary<String, BuildingBody>();

        public MapCompBuildingTracker(Map map) : base(map)
        {
            BodyOverlayHandler.bodiesHandlers.Add(this);
        }

        public override void MapRemoved()
        {
            BodyOverlayHandler.bodiesHandlers.Remove(this);
            base.MapRemoved();
        }

        public void Register(CompBuildingCore core)
        {
            BuildingBody body = bodies.TryGetValue(core.bodyId);
            if (body == null)
            {
                body = new BuildingBody();
                bodies.Add(core.bodyId, body);
            }
            body.Register(core);
        }

        public void Register(CompBuildingBodyPart comp)
        {
            BuildingBody body = bodies.TryGetValue(comp.bodyId);
            if (body == null)
            {
                body = new BuildingBody();
                bodies.Add(comp.bodyId, body);
            }
            body.Register(comp);
        }

        public void Register(CompScaffoldConverter converter)
        {
            BuildingBody body = bodies.TryGetValue(converter.bodyId);
            if (body == null)
            {
                body = new BuildingBody();
                bodies.Add(converter.bodyId, body);
            }
            body.Register(converter);
        }

        public void Register(CompNutrition comp)
        {
            BuildingBody body = bodies.TryGetValue(comp.bodyId);
            if (body == null)
            {
                body = new BuildingBody();
                bodies.Add(comp.bodyId, body);
            }
            body.Register(comp);
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (curTick % 120 == 0) { 
                foreach (string b in bodies.Keys)
                {
                    bodies.TryGetValue(b).RunNutrition(120f);
                }
            }
            curTick++;
        }
    }
}