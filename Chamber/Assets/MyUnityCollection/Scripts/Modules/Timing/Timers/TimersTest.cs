

namespace Muc.Timing {

  using System;
  using System.Collections.Generic;
  using Muc.Components.Values;
  using UnityEngine;
  using static Timers;

  public class TimersTest : MonoBehaviour {


    [Serializable]
    public struct Structy {
      public int integer;
      public Interval interval;
    }

    [Serializable]
    public struct Classy {
      public string stringy;
      public FrameInterval frameInterval;
      public Structy nestedStructy;
    }

    public Interval interval;
    public Structy structy;
    public Classy classy;

    public List<Interval> intervalList;
    public List<Timeout> timeoutList = new List<Timeout> { new Timeout(1), new Timeout(3) };

    public List<int> intList;

    public List<GameObject> gameObjectList;
    public List<Structy> structies;
    public List<Classy> classies;
    public List<ValueData> valueDatas;

  }

}