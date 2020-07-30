

namespace Muc.Timing {

  using System;

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A repeating frame based timer which can be used once after each time a duration passes.
    /// </summary>
    [Serializable]
    public class FrameInterval {

      /// <summary>
      /// Represents the ideal frame of last trigger or the frame this FrameInterval was created.
      /// </summary>
      public int start { get; private set; }

      /// <summary>
      /// Frame after start when there is one remaining use.
      /// </summary>
      public int delay {
        get => _delay;
        set {
          if (delay <= 0) throw new ArgumentOutOfRangeException(nameof(delay), $"Value of {nameof(delay)} must be positive.");
          _delay = value;
        }
      }
      [SerializeField]
      private int _delay = 1;

      /// <summary>
      /// Amount of times this FrameInterval has been used.
      /// </summary>
      public int used { get; private set; }

      /// <summary>
      /// Amount of remaining uses.
      /// </summary>
      public int uses => Mathf.FloorToInt((Time.frameCount - start) / delay);


      FrameInterval() { }

      /// <summary>
      /// Creates a frame based repeating timer which can be used after `delay` has passed.
      /// </summary>
      /// <param name="delay">Duration after which Use can be used once in frames</param>
      public FrameInterval(int delay) {
        try {
          this.start = Time.frameCount;
        } catch (UnityException) { }

        this.delay = delay;
      }


      /// <summary>
      /// If there are remaining uses, returns true and consumes one use, otherwise returns false.
      /// </summary>
      /// <returns>Whether the Use was succesful.</returns>
      public bool UseOne() {
        if (Time.frameCount >= start + delay) {
          start = start + delay;
          return true;
        }
        return false;
      }

      /// <summary>
      /// Returns the amount of remaining uses and uses them.
      /// </summary>
      /// <returns>Amount of consumed uses.</returns>
      public int Use() {
        int uses = 0;
        while (Time.frameCount >= start + delay) {
          used++;
          uses++;
          start = start + delay;
        }
        return uses;
      }

      /// <summary>
      /// Runs `action` for each remaining use and depletes the remaining uses.
      /// </summary>
      /// <param name="action">Function called for each remaining use.</param>
      public void Use(Action action) {
        while (Time.frameCount >= start + delay) {
          used++;
          action();
          start = start + delay;
        }
      }
    }

  }

}

#if UNITY_EDITOR
namespace Muc.Timing.Editor {

  using UnityEngine;
  using UnityEditor;
  using static Muc.Timing.Timers;

  public static partial class Timers {

    [CustomPropertyDrawer(typeof(FrameInterval))]
    public class FrameIntervalDrawer : PropertyDrawer {

      public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        var delay = property.FindPropertyRelative("_delay");
        var input = EditorGUI.IntField(position, property.displayName, delay.intValue);
        if (input > 0) delay.intValue = input;

        EditorGUI.EndProperty();
      }

    }

  }

}
#endif
