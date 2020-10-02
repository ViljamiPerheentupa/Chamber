

namespace Game.Paint {

  using System;
  using UnityEngine;
  using Muc.Data.Trees;
  using Muc.Extensions;
  using Unity.Collections;
  using System.Collections.Generic;

  [DefaultExecutionOrder(-1)]
  public partial class ElectricityController : MonoBehaviour {

    public int maxRendered = 5000;
    public float minLineWidth = 2;
    public float maxLineWidth = 3;
    public bool renderTree = true;
    public bool drawNullVoxels = false;

    private static ElectricityController _instance;
    public static ElectricityController instance {
      get {
        if (_instance != null) return _instance;
        if (Application.isEditor) _instance = GameObject.FindObjectOfType<ElectricityController>();
        if (_instance == null) _instance = new GameObject($"{nameof(ElectricityController)}").AddComponent<ElectricityController>();
        _instance.Init();
        return _instance;
      }
    }

    bool inited;
    public VoxelTree<ConductiveData> tree;
    public PaintType conductivePaintType = PaintType.conductive;

    private void Start() {
      // We don't want multiple Controllers
      if (_instance != this && _instance != null) {
        enabled = false;
        Debug.LogWarning($"Singleton {nameof(ElectricityController)} already created. Deleting new instance.");
        Destroy(gameObject);
        return;
      }

      if (!_instance) _instance = this;
      Init();
    }

    void Update() {
      tree = new VoxelTree<ConductiveData>(8);
    }

    private void Init() {
      if (inited) return;
      inited = true;
      tree = new VoxelTree<ConductiveData>(8);
    }

    public ConductiveData GetElectricity(Vector3 position) {
      return null;
    }

    private List<Vector3> ACPTV_vertStorage = new List<Vector3>();
    private List<Vector2> ACPTV_uvStorage = new List<Vector2>();
    private List<int> ACPTV_triStorage = new List<int>();

    public void ApplyConductivity(Paintable ptbl, NativeArray<byte> data, Mesh mesh, int texSize) {

      // Just aliasing long names
      var verts = ACPTV_vertStorage;
      var uvs = ACPTV_uvStorage;
      var tris = ACPTV_triStorage;

      mesh.GetVertices(verts);
      mesh.GetUVs(0, uvs);
      mesh.GetTriangles(tris, 0);

      var painted = new HashSet<ConductiveData>(); // Set of voxel data that was passed

      int i = 0;
      while (i < tris.Count) {

        var v1 = tris[i++];
        var v2 = tris[i++];
        var v3 = tris[i++];

        var uv1 = uvs[v1];
        var uv2 = uvs[v2];
        var uv3 = uvs[v3];

        var ws1 = ptbl.transform.TransformPoint(verts[v1]);
        var ws2 = ptbl.transform.TransformPoint(verts[v2]);
        var ws3 = ptbl.transform.TransformPoint(verts[v3]);

        // Rasterize triangle. Check each pixel for electric paint

        var rect = new Rect();
        rect.xMin = Mathf.Min(uv1.x, uv2.x, uv3.x);
        rect.xMax = Mathf.Max(uv1.x, uv2.x, uv3.x);

        rect.yMin = Mathf.Min(uv1.y, uv2.y, uv3.y);
        rect.yMax = Mathf.Max(uv1.y, uv2.y, uv3.y);

        float aTri = SignedSize(uv1, uv2, uv3); // Area of the wole triangle multiplied by 2

        var step = 1f / texSize;
        for (float x = rect.xMin; x <= rect.xMax; x += step) {
          for (float y = rect.yMin; y <= rect.yMax; y += step) {

            var uvP = new Vector2(x, y);
            var pixelCoord = new Vector2Int((int)(x * texSize), (int)(y * texSize));

            float a3 = SignedSize(uv1, uv2, uvP); if (a3 > 0) continue; // Signed area of the triangle (v1 -> v2 -> uv) multiplied by 2
            float a1 = SignedSize(uv2, uv3, uvP); if (a1 > 0) continue; // Signed area of the triangle (v2 -> v3 -> uv) multiplied by 2
            float a2 = SignedSize(uv3, uv1, uvP); if (a2 > 0) continue; // Signed area of the triangle (v3 -> v1 -> uv) multiplied by 2

            a1 /= aTri; // Total weight of vertex 1
            a2 /= aTri; // Total weight of vertex 2
            a3 /= aTri; // Total weight of vertex 3

            Vector3Int posPix = (a1 * ws1 + a2 * ws2 + a3 * ws3).FloorInt(); // Interpolated position

            if (posPix.x >= tree.length || posPix.y >= tree.length || posPix.z >= tree.length || posPix.x < 0 || posPix.y < 0 || posPix.z < 0) continue;
            var el = tree[posPix];
            if (el == null) el = new ConductiveData();

            var paint = (PaintType)(
              (float)data[
                ptbl.CoordToIndex(ptbl.WrapCoord(pixelCoord.x), ptbl.WrapCoord(pixelCoord.y))
              ] / (float)ptbl.paintStep
            );

            var isConductor = paint == conductivePaintType;

            var voxDat = tree[posPix];
            if (voxDat is null) {
              if (!isConductor) continue;
              voxDat = new ConductiveData();
              tree[posPix] = voxDat;
            }

            var wasPainted = painted.Contains(voxDat);
            if (wasPainted) continue;

            if (!isConductor) {
              voxDat.RemoveSource(mesh);
              continue;
            }
            painted.Add(voxDat);
            voxDat.AddSource(mesh);
          }
        }
      }
    }

    private float SignedSize(Vector2 v1, Vector2 v2, Vector2 v3) {
      return (v1.x - v3.x) * (v2.y - v3.y) - (v2.x - v3.x) * (v1.y - v3.y);
    }
  }

}
