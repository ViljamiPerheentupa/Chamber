using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmmoPickup : MonoBehaviour
{
    public GunContainer.AmmoType ammo;

    public UnityEvent onPickup;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GunContainer gc = other.GetComponent<GunContainer>();
            if (gc) {
                gc.SetAmmoTypeStatus(ammo, true);
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Collectitem");
            onPickup.Invoke();
            Destroy(gameObject);
        }
    }
}
