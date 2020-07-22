
namespace Muc.Components {

  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;

  [DisallowMultipleComponent]
  public class Tags : MonoBehaviour, ICollection<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>, ISerializationCallbackReceiver {


    #region Global Static Container

    private static Dictionary<string, HashSet<Tags>> tagsDict = new Dictionary<string, HashSet<Tags>>();

    public static Tags[] GetComponents(string tag) {
      if (tagsDict.TryGetValue(tag, out var val)) {
        return val.ToArray();
      }
      return new Tags[0];
    }

    public static Tags[] GetComponentsContainingAll(IEnumerable<string> tags) {
      var emumer = tags.GetEnumerator();

      if (!emumer.MoveNext()) return new Tags[0]; // Empty

      var first = emumer.Current;
      if (tagsDict.TryGetValue(first, out var val)) {
        return val.Where(c => c.ContainsAll(tags)).ToArray();
      }

      return new Tags[0];
    }

    public static Tags[] GetComponentsContainingAny(IEnumerable<string> tags) {
      var res = new Tags[0];

      foreach (var tag in tags) {
        if (tagsDict.TryGetValue(tag, out var val)) {
          res.Concat(val).ToArray();
        }
      }

      return res;
    }

    public static GameObject[] GetObjects(string tag) => GetComponents(tag).Select(v => v.gameObject).ToArray();
    public static GameObject[] GetObjectsContainingAll(IEnumerable<string> tags) => GetComponentsContainingAll(tags).Select(v => v.gameObject).ToArray();
    public static GameObject[] GetObjectsContainingAny(IEnumerable<string> tags) => GetComponentsContainingAny(tags).Select(v => v.gameObject).ToArray();

    #endregion



    #region Component
    #region - Functionality

    private HashSet<string> tags = new HashSet<string>();
    private string[] serializationTags;

    public int Count => tags.Count;
    public bool IsReadOnly => false;

    void ICollection<string>.Add(string tag) => Add(tag);
    public bool Add(string tag) {
      RegisterTag(tag);
      return tags.Add(tag);
    }

    public bool Remove(string tag) {
      UnregisterTag(tag);
      return tags.Remove(tag);
    }
    public void Clear() {
      Unregister();
      tags.Clear();
    }

    public bool Contains(string tag) => tags.Contains(tag);
    public bool ContainsAll(IEnumerable<string> tags) => tags.All(tag => Contains(tag));
    public bool ContainsAny(IEnumerable<string> tags) => tags.Any(tag => Contains(tag));

    public bool CompareTags(Tags other) => other.CompareTags(tags);
    public bool CompareTags(HashSet<string> tags) {
      if (tags.Count != this.tags.Count) return false;
      return ContainsAll(tags); // Same tags?
    }


    void ICollection<string>.CopyTo(string[] array, int arrayIndex) => tags.CopyTo(array, arrayIndex);
    public IEnumerator<string> GetEnumerator() => tags.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => tags.GetEnumerator();

    #endregion



    #region - Innerworks

    void OnDestroy() => Unregister();

    void ISerializationCallbackReceiver.OnBeforeSerialize() {
      serializationTags = tags.ToArray();
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() {
      if (serializationTags == null) return;
      tags = new HashSet<string>(serializationTags);
      Register();
    }

    private void Register() {
      foreach (var tag in tags) {
        RegisterTag(tag);
      }
    }
    private void RegisterTag(string tag) {
      if (!tagsDict.ContainsKey(tag)) tagsDict[tag] = new HashSet<Tags>();
      tagsDict[tag].Add(this);
    }


    private void Unregister() {
      foreach (var tag in tags) {
        UnregisterTag(tag);
      }
    }
    private void UnregisterTag(string tag) {
      if (tagsDict.TryGetValue(tag, out var val)) {
        val.Remove(this);
      }
    }


    #endregion
    #endregion


  }

}


#if UNITY_EDITOR
namespace Muc.Components.Editor {

  using System.Linq;

  using UnityEngine;
  using UnityEditor;

  [CustomEditor(typeof(Tags))]
  public class TagsEditor : Editor {

    private string newTagName = "New Tag";

    private Tags t => target as Tags;


    public override void OnInspectorGUI() {
      serializedObject.Update();

      // Add new tag
      using (new EditorGUILayout.HorizontalScope()) {
        newTagName = EditorGUILayout.TextField(newTagName);
        if (GUILayout.Button("Add Tag")) {
          if (!t.Add(newTagName)) {

            var i = 0;
            while (t.Contains($"{newTagName} ({++i})") && i < 9999) { }
            t.Add($"{newTagName} ({i})");
          }
        }
      }

      EditorGUILayout.Space();

      // Display tags and allow removal
      foreach (var tag in t.ToArray()) {

        using (new EditorGUILayout.HorizontalScope()) {
          GUILayout.Label(tag);
          if (GUILayout.Button("Remove Tag")) {
            t.Remove(tag);
          }
        }

      }



      serializedObject.ApplyModifiedProperties();
    }
  }
}
#endif