
namespace global {

  using System;

  using UnityEngine;

  [RequireComponent(typeof(PaintController))]
  [RequireComponent(typeof(Collider))]
  public class ClickPaint : MonoBehaviour {

    public PaintController.PaintType paintType = PaintController.PaintType.none;
    public Vector2 size = new Vector2(0.01f, 0.01f);

    private Collider col;
    private PaintController paint;

    void Start() {
      col = GetComponent<Collider>();
      paint = GetComponent<PaintController>();
    }


    void Update() {
      if (Input.GetKey(KeyCode.Mouse0)) {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject) {
          paint.PaintArea(hit.lightmapCoord - size / 2, size, paintType);
          paint.Apply();
        }
      }
    }
  }

}
