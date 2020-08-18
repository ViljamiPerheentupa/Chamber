

namespace Muc.Components.Values {

  using UnityEngine.Events;


  public class Mana : Value<float, Mana> {

    private const int DEFAULT_VALUE = 100;

    protected override float defaultValue => DEFAULT_VALUE;
    public float max = DEFAULT_VALUE;

    protected override float AddRawToValue(float addition) => value += addition;

    public override void AddToValue(float value) {
      base.AddToValue(value);
    }
  }

  public abstract class ManaModifier : Modifier<float, Mana> { }
}