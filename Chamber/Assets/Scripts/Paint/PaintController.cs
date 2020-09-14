

namespace Game.Paint {

  using System;
  using UnityEngine;
  using Muc.Data.Trees;
  using Muc.Extensions;
  using Unity.Collections;
  using System.Collections.Generic;

  public class PaintController : MonoBehaviour {

    private static object @lock = new object();
    private static PaintController _instance;
    public static PaintController instance {
      get {
        if (_instance != null) return _instance;
        if (Application.isEditor) _instance = GameObject.FindObjectOfType<PaintController>();
        if (_instance == null) _instance = new GameObject($"{nameof(PaintController)}").AddComponent<PaintController>();
        return _instance;
      }
    }

    public VoxelTree<PaintType> tree;

    private void Start() {
      // We don't want multiple Controllers
      if (_instance != this) {
        enabled = false;
        Debug.LogWarning($"Singleton {nameof(PaintController)} already created. Deleting new instance.");
        Destroy(gameObject);
        return;
      } else {
        _instance = this;
        Init();
      }
    }

    private void Init() {
      tree = VoxelTree.Create256<PaintType>();
    }

    public PaintType GetPaint(Vector3 position) {
      return PaintType.none;
    }

    private List<Vector3> ApplyPaintToVoxels_vertStorage = new List<Vector3>();
    private List<Vector2> ApplyPaintToVoxels_uvStorage = new List<Vector2>();
    private List<int> ApplyPaintToVoxels_triStorage = new List<int>();

    public PaintType ApplyPaintToVoxels(Mesh mesh, NativeArray<byte> data, int texSize) {
      // Just aliasing long names
      var verts = ApplyPaintToVoxels_vertStorage;
      var uvs = ApplyPaintToVoxels_uvStorage;
      var tris = ApplyPaintToVoxels_triStorage;

      mesh.GetVertices(verts);
      mesh.GetUVs(0, uvs);
      mesh.GetTriangles(tris, 0);

      int i = 0;
      while (i < tris.Count) {
        var vert1 = tris[i++];
        var vert2 = tris[i++];
        var vert3 = tris[i++];

        var vert1Uv = uvs[vert1];
        var vert2Uv = uvs[vert2];
        var vert3Uv = uvs[vert3];

        var vert1Pos = verts[vert1];
        var vert2Pos = verts[vert2];
        var vert3Pos = verts[vert3];

        // Draw triangle using pixel coords
        // Set each pixel drawn to paint type in the VoxelTree
      }

      return PaintType.none;
    }


    public int CoordToIndex(Vector2Int coord, int texSize) => CoordToIndex(coord.x, coord.y, texSize);
    public int CoordToIndex(int x, int y, int texSize) {
      return y * texSize + x;
    }
  }

}
