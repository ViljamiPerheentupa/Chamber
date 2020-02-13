using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for testing

public class Gun : MonoBehaviour
{
    int[] loadOut = new int[3];
    int chamberIndex = 2;
    int reloadIndex = 0;
    bool isReloading;
    TextMeshProUGUI[] chamberUI;

    void PullTrigger() {
        // Change chamberIndex
        chamberIndex++;
        if(chamberIndex > 2) {
            chamberIndex = 0;
        }
        // Check ammo type and fire correct projectile
        Fire(loadOut[chamberIndex]);

        // Reset ammo slot
        loadOut[chamberIndex] = 0; // Do we want an enum?
        SetChamberUI();
    }

    void Fire(int type) {
        // Todo: Raycast
    }

    void StartReloading() {
        // Todo: Start reload animation
        isReloading = true;

        for(int i = 0; i < loadOut.Length; i++) {
            loadOut[i] = 0;
        }

        SetChamberUI();
    }

    void SetChamberUI() {
        for(int i = 0; i < 3; i++) {
            chamberUI[i].text = "" + loadOut[(i + chamberIndex) % 3];
        }
    }

    void StopReloading() {
        reloadIndex = 0;
        isReloading = false;
    }

    void LoadChamber(int ammo) {
        loadOut[reloadIndex] = ammo;
        reloadIndex++;
        if(reloadIndex > 2) {
            StopReloading();
        }
        SetChamberUI();
    }

    private void Update() {
        if(Input.GetButtonDown("Fire1")) {
            PullTrigger();
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            if(!isReloading)
                StartReloading();
            else
                StopReloading();
        }

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
    private void Start() {
        chamberUI = FindObjectsOfType<TextMeshProUGUI>();
    }
}