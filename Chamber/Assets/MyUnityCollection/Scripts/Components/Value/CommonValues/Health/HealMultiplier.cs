

namespace Muc.Components.Values {

  using UnityEngine;


  public class HealMultiplier : HealModifier {

    [field: SerializeField]
    float multiplier { get; set; }

    public override float ModifyHeal(float current, Health value) {
      return current * multiplier;
    }
  }
}