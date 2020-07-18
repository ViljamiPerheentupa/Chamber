﻿
namespace global {

  using System;

  using Unity.Collections;
  using UnityEngine;
  using UnityEngine.Experimental.Rendering;

  [RequireComponent(typeof(Renderer))]
  public class PaintController : MonoBehaviour {

    public enum PaintType : byte {
      none = 0,
      paint1 = 1,
      paint2 = 2,
      paint3 = 3,
      paint4 = 4,
      paint5 = 5,
      paint6 = 6,
      paint7 = 7,
      paint8 = 8,

      ignore = 254,
    }

    public static readonly int textureSize = 512;

    [Tooltip("Create texture at Start")]
    public bool preInitialize = false;

    private Material mat;
    private Texture2D tex;
    private NativeArray<byte> data;

    private byte paintStep;

    private bool paint;

    void Start() {
      mat = GetComponent<Renderer>().material;
      if (preInitialize) Init();
    }

    void Init() {
      tex = new Texture2D(textureSize, textureSize, GraphicsFormat.R8_UNorm, TextureCreationFlags.None);
      paintStep = (byte)(byte.MaxValue / (byte)mat.GetInt("Paint_Texture_Count"));
      data = tex.GetRawTextureData<byte>();
      Clear();
      mat.SetTexture("Paint_Index_Map", tex);
    }


    void EnablePaint() {
      if (!paint) {
        paint = true;
        mat.SetFloat("PAINT", 1);
        if (!tex) {
          Init();
        }
      }
    }

    void DisablePaint(bool destroyTexture = true) {
      if (paint) {
        mat.SetFloat("PAINT", 0);
        if (destroyTexture) {
          tex = null;
          mat.SetTexture("Paint_Index_Map", tex);
        }
      }
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void Paint(Vector2 uv, PaintType paint) {
      Paint((int)(uv.x * textureSize), (int)(uv.y * textureSize), paint);
    }
    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void Paint(int x, int y, PaintType paint) {
      if (paint == PaintType.ignore) return;
      EnablePaint();

      data[CoordToIndex(WrapCoord(x), WrapCoord(y))] = (byte)((byte)paint * paintStep);
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(Vector2 uv, Vector2 dims, PaintType paint) {
      PaintArea((int)(uv.x * textureSize), (int)(uv.y * textureSize), (int)(dims.x * textureSize), (int)(dims.y * textureSize), paint);
    }
    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(int x, int y, int w, int h, PaintType paint) {

      if (paint == PaintType.ignore) return;
      EnablePaint();

      // Positivize width and height
      if (w < 0) x -= w *= -1;
      if (h < 0) y -= h *= -1;

      var v = (byte)((byte)paint * paintStep);

      for (int hLoop = 0; hLoop < h; hLoop++) {
        for (int wLoop = 0; wLoop < w; wLoop++) {
          data[CoordToIndex(WrapCoord(wLoop + x), WrapCoord(hLoop + y))] = v;
        }
      }
    }

    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(Vector2 uv, Vector2 dims, PaintType[,] paintData) {
      PaintArea((int)(uv.x * textureSize), (int)(uv.y * textureSize), paintData);
    }
    /// <summary> Call Apply after doing paint operations so the changes are applied </summary>
    public void PaintArea(int x, int y, PaintType[,] paintData) {

      EnablePaint();

      int w = paintData.GetLength(0);
      int h = paintData.GetLength(1);

      for (int hLoop = 0; hLoop < h; hLoop++) {
        for (int wLoop = 0; wLoop < w; wLoop++) {
          var paint = paintData[wLoop, hLoop];
          if (paint != PaintType.ignore)
            data[CoordToIndex(WrapCoord(wLoop + x), WrapCoord(hLoop + y))] = (byte)((byte)paint * paintStep);
        }
      }
    }

    public void Clear() {
      for (int i = 0; i < data.Length; i++) {
        data[i] = (byte)PaintType.none;
      }
    }

    public void Apply() {
      if (tex) tex.Apply();
    }


    int WrapCoord(int a) {
      return (a % textureSize + textureSize) % textureSize;
    }

    public Vector2Int IndexToCoord(int i) {
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
