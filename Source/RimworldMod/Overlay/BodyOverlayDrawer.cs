using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace LivingBuilding
{
	// Token: 0x02000005 RID: 5
	public class BodyOverlayDrawer
	{
		private BuildingBody body = null;
		private Color bodyColor;

		public BodyOverlayDrawer(BuildingBody _body, Color _bodyColor)
		{
			body = _body;
			bodyColor = _bodyColor;
		}

		public void CellBoolDrawerUpdate()
		{
			this.ActuallyDraw();
		}

		private void ActuallyDraw()
		{
			if (this.dirty)
			{
				this.RegenerateMeshFast();
			}
			for (int i = 0; i < this.meshes.Count; i++)
			{
				Graphics.DrawMesh(this.meshes[i], Vector3.zero, Quaternion.identity, this.material, 0);
			}
		}

		public void SetDirty()
		{
			this.dirty = true;
		}

		public void RegenerateMeshFast()
		{
			HashSet<IntVec3> seen = new HashSet<IntVec3>();
			for (int i = 0; i < this.meshes.Count; i++)
			{
				this.meshes[i].Clear();
			}
			int curMesh = 0;
			if (this.meshes.Count < 1)
			{
				Mesh mesh = new Mesh();
				mesh.name = "BodyCellBoolDrawer";
				this.meshes.Add(mesh);
			}
			Mesh mesh2 = this.meshes[curMesh];
			float y = AltitudeLayer.MapDataOverlay.AltitudeFor();
			bool careAboutVertexColors = false;
			foreach (Thing t in this.body.bodyParts)
			{
				foreach (IntVec3 pos in GenAdj.CellsOccupiedBy(t))
                {
					if (!seen.Contains(pos))
                    {
						BodyOverlayDrawer.verts.Add(new Vector3((float)pos.x, y, (float)pos.z));
						BodyOverlayDrawer.verts.Add(new Vector3((float)pos.x, y, (float)(pos.z + 1)));
						BodyOverlayDrawer.verts.Add(new Vector3((float)(pos.x + 1), y, (float)(pos.z + 1)));
						BodyOverlayDrawer.verts.Add(new Vector3((float)(pos.x + 1), y, (float)pos.z));

						BodyOverlayDrawer.colors.Add(bodyColor);
						BodyOverlayDrawer.colors.Add(bodyColor);
						BodyOverlayDrawer.colors.Add(bodyColor);
						BodyOverlayDrawer.colors.Add(bodyColor);

						careAboutVertexColors = true;
						int count = BodyOverlayDrawer.verts.Count;
						BodyOverlayDrawer.tris.Add(count - 4);
						BodyOverlayDrawer.tris.Add(count - 3);
						BodyOverlayDrawer.tris.Add(count - 2);
						BodyOverlayDrawer.tris.Add(count - 4);
						BodyOverlayDrawer.tris.Add(count - 2);
						BodyOverlayDrawer.tris.Add(count - 1);

					}
				}
				if (BodyOverlayDrawer.verts.Count >= 16383)
				{
					this.FinalizeWorkingDataIntoMesh(mesh2);
					curMesh++;
					if (this.meshes.Count < curMesh + 1)
					{
						Mesh mesh3 = new Mesh();
						mesh3.name = "BodyCellBoolDrawer";
						this.meshes.Add(mesh3);
					}
					mesh2 = this.meshes[curMesh];
				}
			}
			this.FinalizeWorkingDataIntoMesh(mesh2);
			this.CreateMaterialIfNeeded(careAboutVertexColors);
			this.dirty = false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002C60 File Offset: 0x00000E60
		private void FinalizeWorkingDataIntoMesh(Mesh mesh)
		{
			if (BodyOverlayDrawer.verts.Count > 0)
			{
				mesh.SetVertices(BodyOverlayDrawer.verts);
				BodyOverlayDrawer.verts.Clear();
				mesh.SetTriangles(BodyOverlayDrawer.tris, 0);
				BodyOverlayDrawer.tris.Clear();
				mesh.SetColors(BodyOverlayDrawer.colors);
				BodyOverlayDrawer.colors.Clear();
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002CC8 File Offset: 0x00000EC8
		private void CreateMaterialIfNeeded(bool careAboutVertexColors)
		{
			if (this.material == null)
			{
				this.material = SolidColorMaterials.SimpleSolidColorMaterial(new Color(bodyColor.r, bodyColor.g, bodyColor.b, this.opacity * bodyColor.a), careAboutVertexColors);
				this.materialCaresAboutVertexColors = careAboutVertexColors;
				this.material.renderQueue = this.renderQueue;
			}
		}

		private Material material;

		private bool materialCaresAboutVertexColors;

		private bool dirty = true;

		private List<Mesh> meshes = new List<Mesh>();

		private float opacity = 0.33f;

		private int renderQueue = 3600;

		private static List<Vector3> verts = new List<Vector3>();

		private static List<int> tris = new List<int>();

		private static List<Color> colors = new List<Color>();

		// Token: 0x04000019 RID: 25
		private const float DefaultOpacity = 0.33f;

		// Token: 0x0400001A RID: 26
		private const int MaxCellsPerMesh = 16383;
	}
}
