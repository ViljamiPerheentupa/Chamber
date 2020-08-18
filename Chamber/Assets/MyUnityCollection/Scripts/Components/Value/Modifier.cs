

namespace Muc.Components.Values {

  using System;
  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;
  using System.Reflection;

  using UnityEngine;
  using UnityEngine.Events;

  using Muc.Types.Extensions;

  public abstract class Modifier<T, TValue> where TValue : Value<T, TValue> {
    public abstract T Apply(T current, TValue value);
  }
}