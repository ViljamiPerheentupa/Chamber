using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    int[] loadOut = new int[3];
    int chamberIndex = 2;
    int reloadIndex = 0;
    bool isReloading;

    void PullTrigger() {
        // Change chamberIndex
        chamberIndex++;
        if(chamberIndex > 2)
            chamberIndex = 0;

        // Check ammo type and fire correct projectile
        Fire(loadOut[chamberIndex]);

        // Reset ammo slot
        loadOut[chamberIndex] = 0; // Do we want an enum?
    }

    void Fire(int type) {
        // Todo: Raycast
    }

    void StartReloading() {
        // Todo: Start reload animation
        isReloading = true;
    }
    void StopReloading() {
        isReloading = false;
    }

    void LoadChamber(int ammo) {
        loadOut[reloadIndex] = ammo;
        reloadIndex++;
        if(reloadIndex > 2) {
            StopReloading();
        }
    }

    private void Update() {
        if(isReloading) {
            if(Input.GetKeyDown("1")) {
                LoadChamber(1);
            }
            if(Input.GetKeyDown("2")) {
                LoadChamber(2);
            }
            if(Input.GetKeyDown("3")) {
                LoadChamber(3);
            }
        }
    }
}
