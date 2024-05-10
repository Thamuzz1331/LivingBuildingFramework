using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using LivingBuildings;

namespace RimWorld
{
    [StaticConstructorOnStartup]
    class Building_Scaffold : Building
    {
        CompScaffold scaffold;
        Graphic g;
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
            if (scaffold == null)
            {
                scaffold = this.TryGetComp<CompScaffold>();
            }
            if (scaffold.transforming)
            {
                if (g == null)
                {
                    g = scaffold.transformDef.graphic;
                }
                float alpha = (scaffold.Props.transformTime - scaffold.transformCountdown) / scaffold.Props.transformTime;
                g.GetColoredVersion(ShaderDatabase.MetaOverlay, new Color(1f, 1f, 1f, alpha), new Color(1f, 1f, 1f, alpha))
                    .Draw(new Vector3(this.DrawPos.x, this.DrawPos.y + 1f, this.DrawPos.z), this.Rotation, this);
            }
        }
    }
}