using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmmoPickup : MonoBehaviour {
    public enum Type {
        AirShotgun,
        Shock,
        Switcheroo,
        Time
    };

    public Type ammo;

    public UnityEvent onPickup;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            AirShotgun air = FindObjectOfType<AirShotgun>();
            Gun gc = FindObjectOfType<Gun>();
            if (gc && air) {
                switch (ammo) {
                case Type.AirShotgun:
                    air.SetStatus(true);
                    break;
                case Type.Shock:
                    gc.SetAmmoTypeStatus(Gun.AmmoType.eShock, true);
                    break;
                case Type.Switcheroo:
                    gc.SetAmmoTypeStatus(Gun.AmmoType.Switcheroo, true);
                    break;
                case Type.Time:
                    gc.SetAmmoTypeStatus(Gun.AmmoType.Time, true);
                    break;
                };
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Collectitem");
            onPickup.Invoke();
            Destroy(gameObject);
        }
    }
}
