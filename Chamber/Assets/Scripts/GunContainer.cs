using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunContainer : MonoBehaviour {
    private const int numTypes = 3;

    public enum AmmoType { Empty, eShock, Magnet, Time };
    public GunAmmoBase[] ammoTypes = new GunAmmoBase[numTypes];
    public bool[] enabledAmmoTypes = new bool[numTypes];
    private Camera cam;
    public float fireCooldownTime = 0.3f;
    private float nextFire = 0.0f;

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        // In cooldown
        if (nextFire > Time.time) {
            return;
        }

        Vector3 pos = cam.transform.position;
        Vector3 fwd = cam.transform.forward;

        if (Input.GetKeyDown("1") && enabledAmmoTypes[0]) {
            ammoTypes[0].Fire(pos, fwd);
            nextFire = Time.time + fireCooldownTime;
        }
        else if (Input.GetKeyDown("2") && enabledAmmoTypes[1]) {
            ammoTypes[1].Fire(pos, fwd);
            nextFire = Time.time + fireCooldownTime;
        }
        else if (Input.GetKeyDown("3") && enabledAmmoTypes[2]) {
            ammoTypes[2].Fire(pos, fwd);
            nextFire = Time.time + fireCooldownTime;
        }
    }

    public void SetAmmoTypeStatus(AmmoType type, bool status) {
        int itype = ((int)type) - 1;
        if (itype >= 0 && itype < numTypes)
            enabledAmmoTypes[itype] = status;
    }
}
