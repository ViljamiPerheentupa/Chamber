
namespace global {

  using System;

  using UnityEngine;

  [RequireComponent(typeof(Paintable))]
  [RequireComponent(typeof(Collider))]
  public class ClickPaint : MonoBehaviour {

    public Paintable.PaintType paintType = Paintable.PaintType.none;
    public Vector2 size = new Vector2(0.01f, 0.01f);

    private Collider col;
    private Paintable paint;

    void Start() {
      col = GetComponent<Collider>();
      paint = GetComponent<Paintable>();
    }


    void Update() {
      if (Input.GetKey(KeyCode.Mouse0)) {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject) {

          // Announce old PaintType
          if (Input.GetKeyDown(KeyCode.Mouse0)) {
            var paintType = paint.GetPaint(hit.lightmapCoord);
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
