

namespace Game.Paint {

  using System;

  using UnityEngine;

  [RequireComponent(typeof(Camera))]
  public class PaintOnClick : MonoBehaviour {

    public PaintType paintType = PaintType.none;
    public Vector2 size = new Vector2(0.01f, 0.01f);

    private Camera cam;

    void Start() {
      cam = GetComponent<Camera>();
    }


    void Update() {
      if (Input.GetKey(KeyCode.Mouse0)) {

        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit)) {
          var hitGo = hit.collider.gameObject;
          if (!hitGo) return;
          var paint = hitGo.GetComponent<Paintable>();
          if (!paint) return;

          // Announce overridden paint type
          if (Input.GetKeyDown(KeyCode.Mouse0)) {
            var paintType = paint.GetPaint(hit.textureCoord);
            Debug.Log($"Painting started on a {paintType} paint pixel");
          }

          // Paint...
          paint.PaintArea(hit.lightmapCoord - size / 2, size, paintType);
          paint.Apply();
        }
      }
    }

  }

}
