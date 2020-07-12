

#if UNITY_EDITOR
namespace Hidden {

  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

  using UnityEngine;
  using UnityEditor;
  using UnityEngine.Experimental.Rendering;

  [CustomEditor(typeof(Texture2DArrayCreator))]
  public class Texture2DArrayCreatorEditor : Editor {

    Texture2D[] sources => t.sources;

    private Texture2DArrayCreator t { get => (Texture2DArrayCreator)target; }

    public override void OnInspectorGUI() {
      serializedObject.Update();

      DrawDefaultInspector();

      if (GUILayout.Button("Generate")) {

        // Validate
        if (!t || t.sources.Length == 0) {
          Debug.LogError("Provide atleast one texture.");
          goto skip;
        }

        var first = t.sources[0];

        if (
          t.sources.Any(e => e.width != first.width) ||
          t.sources.Any(e => e.height != first.height)
        ) {
          Debug.LogError("Texture dimensions must match!");
          goto skip;
        }

        if (t.sources.Any(e => e.graphicsFormat != first.graphicsFormat)) {
          Debug.LogWarning("Texture formats differ, generation may fail.");
        }

        if (t.sources.Any(e => !e.isReadable)) {
          Debug.LogError("Textures must be readable (read write enabled)!");
          goto skip;
        }

        // Generate

        var width = first.width;
        var height = first.height;

        var textureArray = new Texture2DArray(width, height, t.sources.Length, first.graphicsFormat, (TextureCreationFlags)t.creationFlags);

        for (int i = 0; i < t.sources.Length; i++) {
          textureArray.SetPixels(t.sources[i].GetPixels(), i);
        }


        AssetDatabase.CreateAsset(textureArray, $"{t.assetPath}/{t.assetName}");
      }
    skip:

      serializedObject.ApplyModifiedProperties();
    }
  }

}
#endif


namespace Hidden {

  using UnityEngine;

  [System.Flags]
  public enum TextureCreationFlagsClone {
    None = 0,
    MipChain = 1,
    Crunch = 64
  }

  [CreateAssetMenu(fileName = "Texture2DArrayCreator", menuName = "ScriptableObjects/Texture2DArrayCreator", order = 1)]
  public class Texture2DArrayCreator : ScriptableObject {

    public Texture2D[] sources;

    public string assetPath = "Assets/Textures/Untitled Texture Array.asset";
    public string assetName = "Untitled Texture Array.asset";

    public TextureCreationFlagsClone creationFlags = TextureCreationFlagsClone.None;

  }

}
