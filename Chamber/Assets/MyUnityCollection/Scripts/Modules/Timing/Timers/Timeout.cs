

namespace Muc.Timing {

  using System;

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A single-use timer which can be used after a duration passes.
    /// </summary>
    [Serializable]
    public class Timeout {

      /// <summary>
      /// The time when this Timeout was created.
      /// </summary>
      public float start { get; private set; }

      /// <summary>
      /// Duration after start this Timeout can be used.
      /// </summary>
      public float delay {
        get => _delay;
        set {
          if (delay <= 0) throw new ArgumentOutOfRangeException(nameof(delay), $"Value of {nameof(delay)} must be positive.");
          _delay = value;
        }
      }
      [SerializeField]
      private float _delay = 1;

      /// <summary>
      /// Whether this Timeout has been used.
      /// </summary>
      public bool used { get; private set; }

      /// <summary>
      /// Whether this Timeout can currently be used.
      /// </summary>
      public bool usable => !used && Time.time >= start + delay;


      Timeout() { }

      /// <summary>
      /// Creates a single-use timer which can be used after `delay` passes.
      /// </summary>
      /// <param name="delay">Time until this Timeout can be used in seconds.</param>
      public Timeout(float delay) {
        try {
          this.start = Time.time;
        } catch (UnityException) { }

        this.delay = delay;
      }


      /// <summary>
      /// If the Timeout has remaining uses, returns true and consumes the use, otherwise returns false.
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

    [CustomPropertyDrawer(typeof(Timeout))]
    public class TimeoutDrawer : PropertyDrawer {

      public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        var delay = property.FindPropertyRelative("_delay");
        var input = EditorGUI.FloatField(position, property.displayName, delay.floatValue);
        if (input > 0) delay.floatValue = input;

        EditorGUI.EndProperty();
      }

    }

  }

}
#endif
