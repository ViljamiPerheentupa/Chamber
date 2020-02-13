using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Gun : MonoBehaviour
{
    public enum AmmoType { Empty, Fire, Water, Air };

    public AmmoType[] loadOut = new AmmoType[3];
    public int chamberIndex = 0;
    public bool isReloading;
    public Transform crosshair;
    public Image[] chamberUI;

    void PullTrigger() {

        if(isReloading) {
            StopReloading();
        }

        // Check ammo type and fire correct projectile
        Fire(loadOut[chamberIndex]);

        // Reset ammo slot
        SetChamberLoad(chamberIndex, AmmoType.Empty);

        // Check if empty 
        if(IsEmpty()) {
            StartReloading();
        } else
            chamberIndex++;
    }

    void Fire(AmmoType type) {
        // Todo: Raycast
    }

    void ResetUICylinder() {
        chamberIndex = 0;
        RotateUICylinder(true);
    }

    void RotateUICylinder(bool reset) {
        if(reset) {
            if(isReloading) {
                crosshair.rotation = Quaternion.Euler(0, 0, 15);
            } else {
                crosshair.rotation = Quaternion.Euler(0, 0, 0);
            }
        } else {
            crosshair.Rotate(0, 0, -120, Space.Self);
        }    
    }

    void SetChamberLoad(int chamber, AmmoType ammo) {
        // Set AmmoType to loadOut
        loadOut[chamberIndex] = ammo;

        // Set Crosshair slot color
        if(ammo == AmmoType.Empty)
            chamberUI[chamberIndex].color = new Color(.5f, .5f, .5f, .5f);
        if(ammo == AmmoType.Fire)
            chamberUI[chamberIndex].color = new Color(1, 0, 0, 1);
        if(ammo == AmmoType.Water)
            chamberUI[chamberIndex].color = new Color(0, 1, 0, 1);
        if(ammo == AmmoType.Air)
            chamberUI[chamberIndex].color = new Color(0, 0, 1, 1);

        // Rotate cylinder
        RotateUICylinder(false);
    }

    void StartReloading() {
        // Todo: Start reload animation
        isReloading = true;
        ResetUICylinder();

        for(int i = 0; i < loadOut.Length; i++) {
            loadOut[i] = AmmoType.Empty;
        }
    }

    void StopReloading() {
        // Todo: Stop loading animation here
        isReloading = false;
        ResetUICylinder();
    }

    void LoadChamber(AmmoType ammo) {
        SetChamberLoad(chamberIndex, ammo);
        chamberIndex++;
        if(chamberIndex > 2) {
            StopReloading();
        }
    }

    bool IsEmpty() {
        bool empty = true;
        foreach(var ammo in loadOut) {
            if(ammo == AmmoType.Empty) {
            } else { empty = false; }
        }
        return empty;
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
                LoadChamber(AmmoType.Fire);
            }
            if(Input.GetKeyDown("2")) {
                LoadChamber(AmmoType.Water);
            }
            if(Input.GetKeyDown("3")) {
                LoadChamber(AmmoType.Air);
            }
        }
    }
    private void Start() {
        crosshair = GameObject.Find("Crosshair").transform;
    }
}