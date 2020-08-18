

namespace Muc.Components.Values {

  using UnityEngine;


  public abstract class DamageModifier : HealthModifier {

    public abstract float ModifyDamage(float current, Health value);

    public sealed override float Apply(float current, Health value) {
      if (current < 0) return ModifyDamage(current, value);
      return current;
    }
  }
}