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
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SwitchE");
            return;
        }
        if (type == Gun.AmmoType.eShock && triggerType == TriggerType.Shockable && turnedOn) {
            onTriggerOff.Invoke();
            turnedOn = false;
            return;
        }
        if (type == Gun.AmmoType.Piercing && triggerType == TriggerType.Normal && !turnedOn) {
            onTriggerOn.Invoke();
            turnedOn = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Switch");
            return;
        }
        if (type == Gun.AmmoType.Piercing && triggerType == TriggerType.Normal && turnedOn) {
            onTriggerOff.Invoke();
            turnedOn = false;
            return;
        }
    }
}
