

#if UNITY_EDITOR
namespace Game.Paint {

  using System;
  using System.Collections.Generic;
  using UnityEditor;
  using UnityEngine;

  [CustomEditor(typeof(ElectricityController))]
  internal class ElectricityControllerEditor : Editor {

    private ElectricityController t => (ElectricityController)target;

    protected virtual void OnSceneGUI() {
      if (t.tree != null && Event.current.GetTypeForControl(0) == EventType.Repaint) {
        Draw();
      }
    }


    void Draw() {

      var e = t.tree.GetDetailedEnumerator();
      int i = 0;
      while (e.MoveNext() && i++ < t.maxRendered) {

        var color = Color.white;
        color.a /= (e.debth * 2 + 1);
        var width = Mathf.Lerp(t.maxLineWidth, t.minLineWidth, (e.debth / (float)(t.tree.debth + 1)));

        var origin = t.transform.position + new Vector3(e.currentOrigin.x * t.transform.lossyScale.x, e.currentOrigin.y * t.transform.lossyScale.y, e.currentOrigin.z * t.transform.lossyScale.z);
        var size = e.currentSize * t.transform.lossyScale;

        if (t.renderTree) {
          if (e.Current.isLeaf) {
            DrawLeaf(origin, size, color, width);
          } else {
            if (i == 1) DrawLeaf(origin, size, color, width);
            DrawParent(origin, size, color, width);
          }
        }

        if (e.debth < t.tree.debth) continue;

        if (!t.drawNullVoxels && e.Current.data == null) continue;

        var voxelColor = e.Current.data == null ? (Color.red * 0.25f) : Color.yellow;
        DrawVoxel(origin, size, voxelColor);
      }
    }

    private void DrawVoxel(Vector3 origin, Vector3 size, Color color) {
      var prevColor = Handles.color;
      Handles.color = color;
      Handles.CubeHandleCap(0, origin + size / 2, Quaternion.identity, size.x, EventType.Repaint);
      Handles.color = prevColor;
    }


    void DrawLeaf(Vector3 origin, Vector3 size, Color color, float width) {
      var prevColor = Handles.color;
      Handles.color = color;

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 0),
        origin + new Vector3(size.x * 1, size.y * 0, size.z * 0),
        origin + new Vector3(size.x * 1, size.y * 1, size.z * 0),
        origin + new Vector3(size.x * 0, size.y * 1, size.z * 0),
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 0),
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 1),
        origin + new Vector3(size.x * 0, size.y * 1, size.z * 1),
        origin + new Vector3(size.x * 1, size.y * 1, size.z * 1),
        origin + new Vector3(size.x * 1, size.y * 0, size.z * 1),
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 1)
      );

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 1, size.y * 0, size.z * 1),
        origin + new Vector3(size.x * 1, size.y * 0, size.z * 0)
      );

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 0, size.y * 1, size.z * 1),
        origin + new Vector3(size.x * 0, size.y * 1, size.z * 0)
      );

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 1, size.y * 1, size.z * 1),
        origin + new Vector3(size.x * 1, size.y * 1, size.z * 0)
      );

      Handles.color = prevColor;
    }

    void DrawParent(Vector3 origin, Vector3 size, Color color, float width) {
      var prevColor = Handles.color;
      Handles.color = color;

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 0.5f, size.y * 0, size.z * 0),
        origin + new Vector3(size.x * 0.5f, size.y * 1, size.z * 0),
        origin + new Vector3(size.x * 0.5f, size.y * 1, size.z * 1),
        origin + new Vector3(size.x * 0.5f, size.y * 0, size.z * 1),
        origin + new Vector3(size.x * 0.5f, size.y * 0, size.z * 0)
      );

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 0, size.y * 0.5f, size.z * 0),
        origin + new Vector3(size.x * 1, size.y * 0.5f, size.z * 0),
        origin + new Vector3(size.x * 1, size.y * 0.5f, size.z * 1),
        origin + new Vector3(size.x * 0, size.y * 0.5f, size.z * 1),
        origin + new Vector3(size.x * 0, size.y * 0.5f, size.z * 0)
      );

      Handles.DrawAAPolyLine(width,
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 0.5f),
        origin + new Vector3(size.x * 1, size.y * 0, size.z * 0.5f),
        origin + new Vector3(size.x * 1, size.y * 1, size.z * 0.5f),
        origin + new Vector3(size.x * 0, size.y * 1, size.z * 0.5f),
        origin + new Vector3(size.x * 0, size.y * 0, size.z * 0.5f)
      );

      Handles.color = prevColor;
    }


  }

}
#endif