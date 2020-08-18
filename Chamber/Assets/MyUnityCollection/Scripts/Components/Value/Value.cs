

namespace Muc.Components.Values {

  using System;
  using System.Linq;
  using System.Collections;
  using System.Collections.Generic;
  using System.Reflection;

  using UnityEngine;
  using UnityEngine.Events;

  using Muc.Types.Extensions;


  /// <summary>
  /// The type parameter `This` should be type you are declaring.  
  /// E.G. `class MyHealth : Value&lt;float, MyHealth&gt; { ... }` 
  /// </summary>
  public abstract class Value<T, This> : MonoBehaviour,
                                         IReadOnlyCollection<Modifier<T, This>>,
                                         IEnumerable<Modifier<T, This>>
                                         where This : Value<T, This> {


    [SerializeField] protected ValueData valueData;

    protected virtual T defaultValue { get; }
    [SerializeField] protected T _value;
    public virtual T value { get => _value; set => _value = value; }

    [SerializeReference]
    private List<object> modifiers = new List<object>() { new Crit(), new DamageMultiplier() };


    protected virtual void Reset() {
      // Add ValueData from other Value Components
      _value = defaultValue;
      if (!valueData) {
        foreach (var mono in FindObjectsOfType<MonoBehaviour>()) {
          if (mono == this) continue;
          if (mono.GetType().IsGenericTypeOf(typeof(Value<,>))) {
            var type = mono.GetType();
            var field = type.GetField(nameof(valueData), BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) continue;
            var val = field.GetValue(mono);
            valueData = val as ValueData;
            // Continue if vd is still null
            if (valueData) break;
          }
        }
      }
    }

    public virtual void AddToValue(T value) {
      var res = value;
      foreach (Modifier<T, This> modifier in modifiers) {
        res = modifier.Apply(res, (This)this);
      }
      this._value = AddRawToValue(res);
    }

    protected abstract T AddRawToValue(T addition);

    protected virtual void AddModifier<TModifier>() where TModifier : Modifier<T, This>, new()
      => AddModifier(new TModifier());

    protected virtual void AddModifier(Modifier<T, This> modifier) {
      var types = valueData.GetModifiers<This>();
      var priority = types.IndexOf(modifier.GetType());
      if (priority == -1) {
        modifiers.Add(modifier);
        Debug.LogWarning($"{modifier.GetType().FullName} was not found in the modifier type list. It was added at the end of the list.");
        return;
      }

      for (int i = 0; i < modifiers.Count; i++) {
        var otherModifier = modifiers[i];
        var otherPriority = types.IndexOf(otherModifier.GetType());
        if (otherPriority < priority) {
          modifiers.Insert(i, modifier);
          return;
        }
      }
      modifiers.Add(modifier);
    }

    internal virtual void RemoveModifier(Modifier<T, This> modifier) {
      modifiers.Remove(modifier);
    }


    #region Interfaces implementation

    // Props
    public int Count => modifiers.Count;

    // Enumerate
    public IEnumerator<Modifier<T, This>> GetEnumerator() {
      foreach (var modifier in modifiers) {
        yield return (Modifier<T, This>)modifier;
      }
    }
    IEnumerator IEnumerable.GetEnumerator() => modifiers.GetEnumerator();


    #endregion
  }
}