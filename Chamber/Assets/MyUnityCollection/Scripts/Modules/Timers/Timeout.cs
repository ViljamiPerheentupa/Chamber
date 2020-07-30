

namespace Muc.Timing {

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A single-use timer which can be used after a duration passes.
    /// </summary>
    public class Timeout {

      /// <summary>
      /// The time when this Timeout was created.
      /// </summary>
      public float start { get; private set; }

      /// <summary>
      /// Duration after start this Timeout can be used.
      /// </summary>
      public float delay { get; private set; }

      /// <summary>
      /// Whether this Timeout has been used.
      /// </summary>
      public bool used { get; private set; }

      /// <summary>
      /// Whether this Timeout can currently be used.
      /// </summary>
      public bool usable => !used && Time.time >= start + delay;


      /// <summary>
      /// Creates a single-use timer which can be used after `delay` passes.
      /// </summary>
      /// <param name="delay">Time until this Timeout can be used in milliseconds.</param>
      public Timeout(float delay) {
        this.start = Time.time;
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