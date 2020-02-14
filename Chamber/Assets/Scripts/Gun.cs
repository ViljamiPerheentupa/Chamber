using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Gun : MonoBehaviour
{
    public enum AmmoType { Empty, Fire, Water, Air };
    public AmmoType[] loadout = new AmmoType[3];
    public int chamberIndex = 0;
    public bool isReloading;
    public bool rotate;
    Quaternion targetRot;
    Quaternion originalRot;
    public Transform crosshair;
    public Image[] chamberUI;
    public float rotateDuration = .12f;
    float rotateTimer = 0;
    public AnimationCurve rotateCurve;

    void PullTrigger() {

        if(isReloading) {
            StopReloading();
        }

        if(rotate) {
            crosshair.rotation = targetRot;
            rotate = false;
        }

        // Check ammo type and fire correct projectile
        Fire(loadout[chamberIndex]);

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
        originalRot = crosshair.rotation;
        rotateTimer = 0;
        if(reset) {
            if(isReloading) {
                print("reset to loadmode");
                //crosshair.rotation = Quaternion.Euler(0, 0, 15);
                targetRot = Quaternion.Euler(0, 0, 15);
                rotate = true;
            } else {
                print("reset to shootmode");
                //crosshair.rotation = Quaternion.Euler(0, 0, 0);
                targetRot = Quaternion.Euler(0, 0, 0);
                rotate = true;
            }
        } else {
            print("flip 120 degrees");
            targetRot = Quaternion.Euler(0, 0, -120) * crosshair.rotation;
            rotate = true;
        }
    }

    void SetChamberLoad(int chamber, AmmoType ammo) {
        // Set AmmoType to loadOut
        loadout[chamberIndex] = ammo;

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

        for(int i = 0; i < loadout.Length; i++) {
            loadout[i] = AmmoType.Empty;
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
        foreach(var ammo in loadout) {
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

        if(rotate) {
            rotateTimer += Time.deltaTime;
            var t = rotateTimer / rotateDuration;
            t = rotateCurve.Evaluate(t);
            crosshair.rotation = Quaternion.SlerpUnclamped(originalRot, targetRot, t);
            if(rotateTimer > rotateDuration) {
                rotate = false;
                crosshair.rotation = targetRot;
            }
        }
    }
    private void Start() {
        crosshair = GameObject.Find("Crosshair").transform;
    }
}