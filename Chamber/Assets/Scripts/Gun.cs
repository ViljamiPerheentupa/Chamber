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
    public bool reloadComplete;
    Quaternion targetRot;
    Quaternion originalRot;
    public Transform crosshair;
    public Image[] chamberUI;
    public float reloadDuration = .12f;
    public float shootingDuration = .12f;
    float rotateTimer = 0;
    public AnimationCurve reloadCurve;
    public AnimationCurve shootingCurve;

    void PullTrigger() {

        // Check if empty 
        if(IsEmpty()) {
            StartReloading();
        } else {

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
            chamberIndex++;
        }
    }

    void Fire(AmmoType type) {
        // Todo: Raycast, choose corresponding sound effect 
    }

    void ResetUICylinder() {
        chamberIndex = 0;
        RotateUICylinder(true);
    }

    void RotateUICylinder(bool reset) {
        originalRot = crosshair.rotation;
        rotateTimer = 0;
        if(reset) {
            if(isReloading) { // Resets cylinder to reload mode
                print("Ready to load");
                targetRot = Quaternion.Euler(0, 0, 15);
                rotate = true;
            } else { // Resets cylinder to shoot mode
                print("Ready to shoot");
                targetRot = Quaternion.Euler(0, 0, 0);
                rotate = true;
            }
        } else {
            // Rotates cylinder after loadout or shooting
            targetRot = Quaternion.Euler(0, 0, -120) * crosshair.rotation;
            rotate = true;
        }
    }

    void SetChamberLoad(int chamber, AmmoType ammo) {
        // Set AmmoType to loadOut
        loadout[chamber] = ammo;

        // Set Crosshair slot color
        if(ammo == AmmoType.Empty)
            chamberUI[chamber].color = new Color(.5f, .5f, .5f, .5f);
        if(ammo == AmmoType.Fire)
            chamberUI[chamber].color = new Color(1, 0, 0, 1);
        if(ammo == AmmoType.Water)
            chamberUI[chamber].color = new Color(0, 1, 0, 1);
        if(ammo == AmmoType.Air)
            chamberUI[chamber].color = new Color(0, 0, 1, 1);

        // Rotate cylinder
        RotateUICylinder(false);
    }

    void StartReloading() {
        // Todo: Start reload animation

        for(int i = 0; i < loadout.Length; i++) {
            print(i);
            SetChamberLoad(i, AmmoType.Empty);
            //loadout[i] = AmmoType.Empty;
        }

        isReloading = true;
        ResetUICylinder();
    }

    void StopReloading() {
        // Todo: Stop loading animation here
        isReloading = false;
        print("Stopped reloading");
        reloadComplete = true;
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
            float t; 
            if(isReloading || reloadComplete) {
                t = rotateTimer / reloadDuration;
                t = reloadCurve.Evaluate(t);
                
            } else {
                t = rotateTimer / shootingDuration;
                t = shootingCurve.Evaluate(t);
            }
  
            crosshair.rotation = Quaternion.SlerpUnclamped(originalRot, targetRot, t);
            if(t >= 1) {
                rotate = false;
                crosshair.rotation = targetRot;
                reloadComplete = false;
            }
        }
    }
    private void Start() {
        crosshair = GameObject.Find("Crosshair").transform;
    }
}