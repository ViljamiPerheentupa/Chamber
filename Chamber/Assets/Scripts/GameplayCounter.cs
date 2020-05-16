using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayCounter : BaseResetable {
    public enum Comparison {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterEqual,
        LessEqual
    };

    [System.Serializable]
    public class CounterLevel {
        public int amount;
        public Comparison comparison;
        public UnityEvent OnSatisfy;
        public bool canOnlyFireOnce;
        public bool fired;
    };

    public UnityEvent OnReset;
    public int counterValue = 0;
    public List<CounterLevel> counterLevels = new List<CounterLevel>();
    private int defaultCounterValue;

    void Start() {
        defaultCounterValue = counterValue;
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public void AddAmount(int amount) {
        counterValue += amount;

        for (int i = 0; i < counterLevels.Count; ++i) {
            CounterLevel c = counterLevels[i];

            switch (c.comparison) {
                default:
                case Comparison.Equal:
                    if (counterValue == c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
                case Comparison.NotEqual:
                    if (counterValue != c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
                case Comparison.Greater:
                    if (counterValue > c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
                case Comparison.GreaterEqual:
                    if (counterValue >= c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
                case Comparison.Less:
                    if (counterValue < c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
                case Comparison.LessEqual:
                    if (counterValue <= c.amount) {
                        if (!(c.canOnlyFireOnce && c.fired)) {
                            c.fired = true;
                            c.OnSatisfy.Invoke();
                        }
                    }
                    break;
            }
        }
    }

    public void SubtractAmount(int amount) {
        AddAmount(-amount);
    }

    public override void StartReset() {
        counterValue = defaultCounterValue;

        for (int i = 0; i < counterLevels.Count; ++i) {
            CounterLevel c = counterLevels[i];
            c.fired = false;
        }
        
        OnReset.Invoke();
    }
}
