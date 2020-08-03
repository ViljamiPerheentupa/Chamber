﻿

namespace Muc.Timing {

  using System;

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A repeating frame based timer which can be used once after each time a duration passes.
    /// </summary>
    [Serializable]
    public class FrameInterval {

      [SerializeField]
      internal int start;
      private int pauseAdjustedStart => (paused ? start + Time.frameCount - pauseTime : start);

      /// <summary>
      /// Frame after start when there is one remaining use.
      /// </summary>
      public int delay {
        get => _delay;
        set {
          if (value <= 0) throw new ArgumentOutOfRangeException(nameof(delay), $"Value of {nameof(delay)} must be positive.");

          if (paused) {
            paused = false;
            paused = true;
          }

          // Calculates start as such that the remaining uses are carried over and leftover time gives up to 1 use
          var startAfterUse = start + uses * _delay;
          var leftovers = Mathf.Min(Time.frameCount - startAfterUse, value);
          start = Time.frameCount - (uses * value + leftovers);

          _delay = value;
        }
      }
      [SerializeField]
      internal int _delay = 1;

      /// <summary>
      /// Amount of times this FrameInterval has been used.
      /// </summary>
      public int used { get; private set; }

      /// <summary>
      /// Amount of remaining uses.
      /// </summary>
      public int uses => paused ? 0 : Mathf.FloorToInt((Time.frameCount - pauseAdjustedStart) / delay);

      /// <summary>
      /// Whether this Timeout is paused.
      /// </summary>
      public bool paused {
        get => _paused;
        set {
          if (_paused == value) return;
          if (value) {
            pauseTime = Time.frameCount;
          } else {
            start = pauseAdjustedStart;
          }
          _paused = value;
        }
      }
      [SerializeField]
      internal bool _paused;
      [SerializeField]
      internal int pauseTime;


      FrameInterval() { }

      /// <summary>
      /// Creates a frame based repeating timer which can be used after `delay` has passed.
      /// </summary>
      /// <param name="delay">Duration after which Use can be used once in frames</param>
      /// <param name="paused">Whether this FrameTimeout will be created in a paused state.</param>
      public FrameInterval(int delay, bool paused = false) {
        try {
          // Throws if scripting API is unavailable
          this.start = Time.frameCount;
          this.paused = paused;
        } catch (UnityException) {
          this._paused = paused;
        }

        _delay = delay;
      }


      /// <summary>
      /// If there are remaining uses, returns true and consumes one use, otherwise returns false.
      /// </summary>
      /// <returns>Whether the Use was succesful.</returns>
      public bool UseOne() {
        if (Time.frameCount >= pauseAdjustedStart + delay) {
          start += delay;
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
        while (Time.frameCount >= pauseAdjustedStart + delay) {
          used++;
          uses++;
          start += delay;
        }
        return uses;
      }

      /// <summary>
      /// Runs `action` for each remaining use and depletes the remaining uses.
      /// </summary>
      /// <param name="action">Function called for each remaining use.</param>
      public void Use(Action action) {
        while (Time.frameCount >= pauseAdjustedStart + delay) {
          used++;
          action();
          start += delay;
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
    internal class FrameIntervalDrawer : PropertyDrawer {

      public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
      }

      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        using (new EditorGUI.PropertyScope(position, label, property)) {

          var delay = property.FindPropertyRelative(nameof(FrameInterval._delay));
          var paused = property.FindPropertyRelative(nameof(FrameInterval._paused));
          var pauseTime = property.FindPropertyRelative(nameof(FrameInterval.pauseTime));
          var start = property.FindPropertyRelative(nameof(FrameInterval.start));

          var noLabel = label.text is "" && label.image is null;

          // Pause bool (Click handling)
          var pausedRect = new Rect(position);
          if (!noLabel) pausedRect.xMin = pausedRect.xMin + EditorGUIUtility.labelWidth - 15 * (EditorGUI.indentLevel + 1);
          pausedRect.width = 15;
          var inActive = EditorGUI.Toggle(pausedRect, !paused.boolValue);
          var inPaused = !inActive;
          // Handle playmode fingering of pause
          if (inPaused != paused.boolValue && Application.isPlaying) {
            if (inPaused) {
              pauseTime.intValue = Time.frameCount;
            } else {
              start.intValue += Time.frameCount - pauseTime.intValue;
            }
          }
          paused.boolValue = inPaused;

          // Delay value
          var delayRect = new Rect(position);
          if (noLabel) delayRect.xMin = pausedRect.xMax + 2;
          var inDelay = EditorGUI.IntField(delayRect, label, delay.intValue);
          if (inDelay != delay.intValue && inDelay > 0 && Application.isPlaying) {
            var field = fieldInfo.GetValue(property.serializedObject.targetObject);
            if (field is FrameInterval target) target.delay = inDelay;
          }
          if (inDelay > 0) delay.intValue = inDelay;

          // Pause bool (Press down visuals)
          EditorGUI.Toggle(pausedRect, inActive);

        }
      }

    }

  }

}
#endif
