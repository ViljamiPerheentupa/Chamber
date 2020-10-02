

namespace Game.Paint {

  using System;

  using Unity.Collections;
  using UnityEngine;
  using UnityEngine.Experimental.Rendering;

  [RequireComponent(typeof(MeshFilter))]
  [RequireComponent(typeof(MeshRenderer))]
  public class Paintable : MonoBehaviour {

    public int textureSize = 64;
    [Tooltip("Is paint wrapped or clamped to the texture uvs?")]
    public bool wrap = true;

    [Tooltip("Create texture at Start?")]
    public bool preInitialize = false;

    private Mesh mesh;
    private Material mat;
    private Texture2D tex;
    private NativeArray<byte> data;

    public byte paintStep { get; private set; }

    private bool paintEnabled;

    void Start() {
      mesh = GetComponent<MeshFilter>().mesh;
      mat = GetComponent<MeshRenderer>().material;
      if (preInitialize) Init();
    }

    void Init() {
      try {
        tex = new Texture2D(textureSize, textureSize, GraphicsFormat.R8_UNorm, TextureCreationFlags.None);
        paintStep = (byte)(byte.MaxValue / (byte)mat.GetInt("Paint_Texture_Count"));
        data = tex.GetRawTextureData<byte>();
        Clear();
        mat.SetTexture("Paint_Index_Map", tex);
      } catch (Exception) {
        Debug.LogError("Failed to initialize the Paintable. Have you added a Material using the Paintable shader?");
        throw;
      }
    }

    void Update() {
      Apply();
    }

    void EnablePaint() {
      if (!paintEnabled) {
        paintEnabled = true;
        mat.EnableKeyword("PAINT_ON");
        mat.DisableKeyword("PAINT_OFF");
        if (!tex) {
          Init();
        }
      }
    }

    void DisablePaint(bool destroyTexture = true) {
      if (paintEnabled) {
        paintEnabled = false;
        mat.DisableKeyword("PAINT_ON");
        mat.EnableKeyword("PAINT_OFF");
        if (destroyTexture) {
          tex = null;
          mat.SetTexture("Paint_Index_Map", tex);
        }
      }
    }

    public PaintType GetPaint(Vector2 uv) => GetPaint((int)(uv.x * textureSize), (int)(uv.y * textureSize));
    public PaintType GetPaint(int x, int y) { // (byte)((byte)paint * paintStep)
      if (!paintEnabled) return PaintType.none;
      return (PaintType)((float)data[CoordToIndex(WrapCoord(x), WrapCoord(y))] / (float)paintStep);
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void Paint(Vector2 uv, PaintType paint) => Paint((int)(uv.x * textureSize), (int)(uv.y * textureSize), paint);
    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void Paint(int x, int y, PaintType paint) {
      if (paint == PaintType.ignore) return;
      EnablePaint();

      data[CoordToIndex(WrapCoord(x), WrapCoord(y))] = (byte)((byte)paint * paintStep);
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(Rect area, PaintType paint) {

      if (paint == PaintType.ignore) return;
      EnablePaint();

      int xMin = (int)(area.xMin * textureSize);
      int yMin = (int)(area.yMin * textureSize);

      int xMax = (int)(area.xMax * textureSize);
      int yMax = (int)(area.yMax * textureSize);

      var v = (byte)((byte)paint * paintStep);

      if (wrap) {
        for (int x = xMin; x < xMax; x++) {
          for (int y = yMin; y < yMax; y++) {
            data[CoordToIndex(WrapCoord(x), WrapCoord(y))] = v;
          }
        }
      } else {
        for (int x = xMin; x < xMax; x++) {
          for (int y = yMin; y < yMax; y++) {
            data[CoordToIndex(ClampCoord(x), ClampCoord(y))] = v;
          }
        }
      }
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(Vector2 origin, PaintType[,] paintData) {

      EnablePaint();

      int x = (int)(origin.x * textureSize);
      int y = (int)(origin.y * textureSize);

      int w = paintData.GetLength(0);
      int h = paintData.GetLength(1);

      if (wrap) {
        for (int hIndex = 0; hIndex < h; hIndex++) {
          for (int wIndex = 0; wIndex < w; wIndex++) {
            var paint = paintData[wIndex, hIndex];
            if (paint != PaintType.ignore) data[CoordToIndex(WrapCoord(wIndex + x), WrapCoord(hIndex + y))] = (byte)((byte)paint * paintStep);
          }
        }
      } else {
        for (int hIndex = 0; hIndex < h; hIndex++) {
          for (int wIndex = 0; wIndex < w; wIndex++) {
            var paint = paintData[wIndex, hIndex];
            if (paint != PaintType.ignore) data[CoordToIndex(ClampCoord(wIndex + x), ClampCoord(hIndex + y))] = (byte)((byte)paint * paintStep);
          }
        }
      }
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void Clear() {
      for (int i = 0; i < data.Length; i++) {
        data[i] = (byte)PaintType.none;
      }
    }

    public void Apply() {
      if (!tex) return;
      tex.Apply();
      ElectricityController.instance.ApplyConductivity(this, data, mesh, textureSize);
    }


    public int WrapCoord(int a) {
      return (a % textureSize + textureSize) % textureSize;
    }

    public int ClampCoord(int a) {
      return Mathf.Clamp(a, 0, paintStep);
    }

    Vector2Int IndexToCoord(int i) {
      var res = Vector2Int.zero;
      res.y = Mathf.FloorToInt(i / textureSize);
      res.x = i % textureSize;
      return res;
    }

    public int CoordToIndex(Vector2Int coord) => CoordToIndex(coord.x, coord.y);
    public int CoordToIndex(int x, int y) {
      return y * textureSize + x;
    }


  }

}