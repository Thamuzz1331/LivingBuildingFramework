using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    public class MapCompBuildingTracker : MapComponent
    {
        int curTick = 0;
        public Dictionary<String, BuildingBody> bodies = new Dictionary<String, BuildingBody>();
        public static String defaultBodyId = "Default";

        public MapCompBuildingTracker(Map map) : base(map)
        {
            BodyOverlayHandler.bodiesHandlers.Add(this);
        }

        public override void MapRemoved()
        {
            BodyOverlayHandler.bodiesHandlers.Remove(this);
            base.MapRemoved();
        }

        public void RegisterCore(CompBuildingCore core)
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
            BuildingBody body = bodies.TryGetValue(comp.bodyId, null);
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

        public void Register(CompScaffold scaff)
        {
            BuildingBody body = bodies.TryGetValue(scaff.bodyId);
            if (body == null)
            {
                body = new BuildingBody();
                bodies.Add(scaff.bodyId, body);
            }
            body.Register(scaff);
        }

        public void Terminate(string bodyId, DestroyMode mode, Map previousMap)
        {
            BuildingBody body = bodies.TryGetValue(bodyId);
            if (body != null)
            {
                body.TerminateBody(mode, previousMap);
                this.bodies.Remove(bodyId);
            }
        }

        public void RepairPulse()
        {
            foreach (String key in this.bodies.Keys)
            {
                if (key != "NA")
                {
                    this.bodies[key].heart.RepairAdjacent();
                }
            }
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            bool nutritionTick = curTick % 120 == 0;
            bool bodyPulseTick = curTick % 1000 == 0;
            foreach (BuildingBody b in bodies.Values)
            {
                if (nutritionTick)
                {
                    b.RunNutrition(120f);
                }
                if (bodyPulseTick)
                {
                    b.BodyDetectionPulse();
                }
                foreach (BuildingHediff diff in b.hediffs)
                {
                    diff.Tick();
                }
                b.RunAddictions(1f);
                CompScaffold[] scaffs = new CompScaffold[b.transformingScaff.Count];
                b.transformingScaff.CopyTo(scaffs);
                foreach(CompScaffold scaff in scaffs)
                {
                    scaff.BodyTick();
                }
            }

            curTick++;
        }
    }
}