

namespace Muc.Timing {

  using System;

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A repeating timer which can be used once after each time a duration passes.
    /// </summary>
    public class Interval {

      /// <summary>
      /// Represents the ideal time of last trigger or when this Interval was created.
      /// </summary>
      public float start { get; private set; }

      /// <summary>
      /// Duration after start there is one remaining use.
      /// </summary>
      public float delay { get; private set; }

      /// <summary>
      /// Amount of times this Interval has been used.
      /// </summary>
      public int used { get; private set; }

      /// <summary>
      /// Amount of remaining uses.
      /// </summary>
      public int uses => Mathf.FloorToInt((Time.time - start) / delay);


      /// <summary>
      /// Creates a repeating timer which can be used after `delay` has passed.
      /// </summary>
      /// <param name="delay">Duration after which Use can be used once in milliseconds.</param>
      public Interval(float delay) {
        this.start = Time.time;
        this.delay = delay;
      }


      /// <summary>
      /// If there are remaining uses, returns true and consumes one use, otherwise returns false.
      /// </summary>
      /// <returns>Whether the Use was succesful.</returns>
      public bool UseOne() {
        if (Time.time >= start + delay) {
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
        while (Time.time >= start + delay) {
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
        while (Time.time >= start + delay) {
          used++;
          action();
          start = start + delay;
        }
      }
    }

  }

}