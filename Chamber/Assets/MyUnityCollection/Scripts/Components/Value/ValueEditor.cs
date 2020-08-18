
#if UNITY_EDITOR
namespace Muc.Components.Values {

  using System;

  using UnityEngine;
  using UnityEditor;
  using System.Reflection;
  using UnityEngine.UIElements;

  [CustomEditor(typeof(Value<,>), true)]
  public class ValueDrawer : Editor {

    SerializedProperty modifiers;

    void OnEnable() {
      modifiers = serializedObject.FindProperty("modifiers");
    }

    public override void OnInspectorGUI() {
      using (new EditorGUI.ChangeCheckScope()) {
        serializedObject.UpdateIfRequiredOrScript();

        DrawPropertiesExcluding(serializedObject, modifiers.name);
        EditorGUILayout.PropertyField(modifiers, true);

        serializedObject.ApplyModifiedProperties();
      }
    }
  }
}
#endif