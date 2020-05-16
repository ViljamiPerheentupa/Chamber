using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmmoPickup : MonoBehaviour {
    public enum Type {
        AirShotgun,
        Shock,
        Magnet,
        Time
    };

    public Type ammo;

    public UnityEvent onPickup;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            AirShotgun air = FindObjectOfType<AirShotgun>();
            GunContainer gc = FindObjectOfType<GunContainer>();
            if (gc && air) {
                switch (ammo) {
                case Type.AirShotgun:
                    air.isActivated = true;
                    break;
                case Type.Shock:
                    gc.SetAmmoTypeStatus(GunContainer.AmmoType.eShock, true);
                    break;
                case Type.Magnet:
                    gc.SetAmmoTypeStatus(GunContainer.AmmoType.Magnet, true);
                    break;
                case Type.Time:
                    gc.SetAmmoTypeStatus(GunContainer.AmmoType.Time, true);
                    break;
                };
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Collectitem");
            onPickup.Invoke();
            Destroy(gameObject);
        }
    }
}
