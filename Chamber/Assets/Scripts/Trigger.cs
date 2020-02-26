using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public enum TriggerType { Collision, Shockable, Normal }
    public UnityEvent onTriggerOn;
    public UnityEvent onTriggerOff;
    public TriggerType triggerType;
    public bool turnedOn = false;

    private void Start() {
        if (turnedOn) {
            onTriggerOn.Invoke();
        }
    }
    void Update()
    {
        
    }

    public void ShootTrigger(Gun.AmmoType type) {
        if (type == Gun.AmmoType.eShock && triggerType == TriggerType.Shockable && !turnedOn) {
            onTriggerOn.Invoke();
            turnedOn = true;
            return;
        }
        if (type == Gun.AmmoType.eShock && triggerType == TriggerType.Shockable && turnedOn) {
            onTriggerOff.Invoke();
            turnedOn = false;
            return;
        }
    }
}
