

namespace Muc.Components.Values {

  using UnityEngine.Events;


  public class Stamina : Value<float, Stamina> {

    private const int DEFAULT_VALUE = 100;

    protected override float defaultValue => DEFAULT_VALUE;
    public float max = DEFAULT_VALUE;

    protected override float AddRawToValue(float addition) => value += addition;

    public override void AddToValue(float value) {
      base.AddToValue(value);
    }
  }

  public abstract class StaminaModifier : Modifier<float, Stamina> { }
}