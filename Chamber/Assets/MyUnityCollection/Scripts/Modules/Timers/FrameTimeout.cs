

namespace Muc.Timing {

  using UnityEngine;

  public static partial class Timers {

    /// <summary>
    /// A single-use frame based timer which can be used after a duration passes.
    /// </summary>
    public class FrameTimeout {

      /// <summary>
      /// Frame when this FrameTimeout was created.
      /// </summary>
      public int start { get; private set; }

      /// <summary>
      /// Frames after start this FrameTimeout can be used.
      /// </summary>
      public int delay { get; private set; }

      /// <summary>
      /// Whether this FrameTimeout has been used.
      /// </summary>
      public bool used { get; private set; }

      /// <summary>
      /// Whether this FrameTimeout can currently be used.
      /// </summary>
      public bool usable => !used && Time.frameCount >= start + delay;


      /// <summary>
      /// Creates a single-use frame based timer which can be used after `delay` passes.
      /// </summary>
      /// <param name="delay">Frames until this FrameTimeout can be used in milliseconds.</param>
      public FrameTimeout(int delay) {
        this.start = Time.frameCount;
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