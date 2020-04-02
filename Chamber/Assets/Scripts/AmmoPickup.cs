using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmmoPickup : MonoBehaviour
{
    public Gun.AmmoType ammo;

    public UnityEvent onPickup;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (ammo == Gun.AmmoType.Piercing) {
                GameObject.Find("Gunvas").GetComponent<Gun>().hasNormal = true;
            }
            if (ammo == Gun.AmmoType.eShock) {
                GameObject.Find("Gunvas").GetComponent<Gun>().hasShock = true;
            }
            if (ammo == Gun.AmmoType.AirBlast) {
                GameObject.Find("Gunvas").GetComponent<Gun>().hasAirburst = true;
            }
            onPickup.Invoke();
            Destroy(gameObject);
        }
    }
}
