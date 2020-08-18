

namespace Muc.Components.Values {

  using System;
  using System.Collections.Generic;

  using UnityEngine;
  using Random = UnityEngine.Random;

  using Muc.Types.Extensions;

  public class Crit : DamageModifier {

    enum CritStacking {
      [Tooltip("The highest multiplier of the triggered crits is used")]
      Highest,
      [Tooltip("Multipliers of triggered crits are combined using addition")]
      Additive,
      [Tooltip("Multipliers of triggered crits are combined using multiplication")]
      Multiplicative,
    }

    [field: SerializeField]
    List<CritValue> crits { get; set; } = new List<CritValue>();

    [field: SerializeField]
    CritStacking critStacking { get; set; } = CritStacking.Additive;

    [Serializable]
    public struct CritValue {
      [field: SerializeField, Range(0, 1)]
      public float chance { get; set; }
      [field: SerializeField, Min(0)]
      public float multiplier { get; private set; }
    }

    public override float ModifyDamage(float current, Health value) {
      if (crits.Count == 0) return current;

      var mult = 1f;
      var rand = Random.value;
      switch (critStacking) {

        case CritStacking.Highest:
          var first = true;
          foreach (var crit in crits) {
            if (first || mult < crit.multiplier) {
              if (rand >= crit.chance) {
                if (first) {
                  mult = crit.multiplier;
                  first = false;
                } else {
                  mult = Mathf.Max(mult, crit.multiplier);
                }
              }
            }
          }
          break;

        case CritStacking.Additive:
          foreach (var crit in crits) {
            if (rand >= crit.chance) mult += crit.multiplier;
          }
          break;

        case CritStacking.Multiplicative:
          foreach (var crit in crits) {
            if (rand >= crit.chance) mult *= crit.multiplier;
          }
          break;

      }

      mult = Mathf.Clamp(mult, 0, float.MaxValue);
      return current * mult;
    }
  }
}