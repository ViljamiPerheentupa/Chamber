


using System.Collections.Generic;
using Muc.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Muc.Components.Values {

  public class Mana : Value<float, Mana> {

    private const int DEFAULT_VALUE = 100;

    protected override float defaultValue => DEFAULT_VALUE;
    public float max = DEFAULT_VALUE;

    public UnityEvent<Mana> onDeath;

    protected override float AddRawToValue(float addition) => value += addition;

    public override void AddToValue(float value) {
      var prevVal = this.value;
      base.AddToValue(value);
      if (this.value <= 0 && prevVal > 0) {
        onDeath.Invoke(this);
      }
    }
  }

  public abstract class ManaModifier : Modifier<float, Mana> { }
}