

namespace Muc.Components.Values {

  using UnityEngine;


  public abstract class HealModifier : HealthModifier {

    public abstract float ModifyHeal(float current, Health value);

    public sealed override float Apply(float current, Health value) {
      if (current > 0) return ModifyHeal(current, value);
      return current;
    }
  }
}