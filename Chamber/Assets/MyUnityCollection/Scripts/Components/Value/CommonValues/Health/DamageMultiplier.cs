

namespace Muc.Components.Values {

  using UnityEngine;


  public class DamageMultiplier : DamageModifier {

    [field: SerializeField]
    float multiplier { get; set; }

    public override float ModifyDamage(float current, Health value) {
      if (current < 0) return current * multiplier;
      return current;
    }
  }
}