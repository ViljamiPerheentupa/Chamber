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

    private void OnTriggerEnter(Collider other) {
        if (triggerType == TriggerType.Collision) {
            onTriggerOn.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (triggerType == TriggerType.Collision) {
            onTriggerOff.Invoke();
        }
    }

    public void ShootTrigger(GunContainer.AmmoType type) {
        if (type == GunContainer.AmmoType.eShock && triggerType == TriggerType.Shockable && !turnedOn) {
            onTriggerOn.Invoke();
            turnedOn = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/SwitchE");
            return;
        }
        if (type == GunContainer.AmmoType.eShock && triggerType == TriggerType.Shockable && turnedOn) {
            onTriggerOff.Invoke();
            turnedOn = false;
            return;
        }
        if (type == GunContainer.AmmoType.Magnet && triggerType == TriggerType.Normal && !turnedOn) {
            onTriggerOn.Invoke();
            turnedOn = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Switch");
            return;
        }
        if (type == GunContainer.AmmoType.Time && triggerType == TriggerType.Normal && turnedOn) {
            onTriggerOff.Invoke();
            turnedOn = false;
            return;
        }
    }
}
