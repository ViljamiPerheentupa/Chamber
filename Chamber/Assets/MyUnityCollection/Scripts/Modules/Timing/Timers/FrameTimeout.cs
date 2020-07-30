

namespace Muc.Timing {

  using System;

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A single-use frame based timer which can be used after a duration passes.
    /// </summary>
    [Serializable]
    public class FrameTimeout {

      /// <summary>
      /// Frame when this FrameTimeout was created.
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
      /// Whether this FrameTimeout has been used.
      /// </summary>
      public bool used { get; private set; }

      /// <summary>
      /// Whether this FrameTimeout can currently be used.
      /// </summary>
      public bool usable => !used && Time.frameCount >= start + delay;


      FrameTimeout() { }

      /// <summary>
      /// Creates a single-use frame based timer which can be used after `delay` passes.
      /// </summary>
      /// <param name="delay">Frames until this FrameTimeout can be used in milliseconds.</param>
      public FrameTimeout(int delay) {
        try {
          this.start = Time.frameCount;
        } catch (UnityException) { }

        this.delay = delay;
      }


      /// <summary>
      /// If the FrameTimeout has remaining uses, returns true and consumes the use, otherwise returns false.
      /// </summary>
      /// <returns></returns>
      public bool Use() {
        if (usable) {
          used = true;
          return true;
        }
        return false;
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

    [CustomPropertyDrawer(typeof(FrameTimeout))]
    public class FrameTimeoutDrawer : PropertyDrawer {

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
