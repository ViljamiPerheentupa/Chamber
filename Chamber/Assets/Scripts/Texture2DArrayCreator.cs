

#if UNITY_EDITOR
namespace global.Editor {

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
          EditorUtility.DisplayDialog("Texture Array Creation Error", "Provide atleast one texture.", "OK");
          goto skip;
        }

        var first = t.sources[0];

        if (
          t.sources.Any(e => e.width != first.width) ||
          t.sources.Any(e => e.height != first.height)
        ) {
          EditorUtility.DisplayDialog("Texture Array Creation Error", "Texture dimensions must match!", "OK");
          goto skip;
        }

        if (t.sources.Any(e => e.graphicsFormat != first.graphicsFormat)) {
          Debug.LogWarning("Texture formats differ, generation may fail.");
        }

        if (t.sources.Any(e => !e.isReadable)) {
          EditorUtility.DisplayDialog("Texture Array Creation Error", "Textures must be readable!\nEnable read write in the textures' import screen.", "OK");
          goto skip;
        }

        // Generate

        var width = first.width;
        var height = first.height;


        var path = EditorUtility.SaveFilePanelInProject("Save texture array as Unity Asset", "New Texture Array", "asset", "Choose save location");

        if (path.Length > 0) {

          var textureArray = new Texture2DArray(width, height, t.sources.Length, first.graphicsFormat, (TextureCreationFlags)t.creationFlags);

          for (int i = 0; i < t.sources.Length; i++) {
            textureArray.SetPixels(t.sources[i].GetPixels(), i);
          }

          if (textureArray) AssetDatabase.CreateAsset(textureArray, path);
          EditorGUIUtility.PingObject(textureArray);
        }

      }

    skip:

      serializedObject.ApplyModifiedProperties();
    }
  }

}
#endif


namespace global {

  using UnityEngine;

  [System.Flags]
  public enum TextureCreationFlagsClone {
    None = 0,
    MipChain = 1,
    Crunch = 64
  }

  public class Texture2DArrayCreator : ScriptableObject {

    public Texture2D[] sources;

    public TextureCreationFlagsClone creationFlags = TextureCreationFlagsClone.None;

  }

}
