using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Gun : MonoBehaviour
{
    public enum AmmoType { Empty, Fire, Water, Air };
    AmmoType[] loadout = new AmmoType[3];
    Animator anim;

    int chamberIndex = 0;
    bool isReloading;
    bool animationBuffer;
    bool rotate;
    bool reloadComplete;

    Quaternion targetRot;
    Quaternion originalRot;
    Quaternion bufferRot;

    public Transform crosshair;
    public Image[] chamberUI;

    public float reloadDuration = .12f;
    public float shootingDuration = .12f;
    float rotateTimer = 0;

    public AnimationCurve reloadCurve;
    public AnimationCurve shootingCurve;

    void PullTrigger() {

        // if reloading, stop
        if(isReloading) {
            if(IsEmpty()) {
                // Play empty shot sound
                print("Click");
                return;
            } else {
                StopReloading();
            }
        }

        // If rotating, complete instantly
        if(rotate) {
            crosshair.rotation = targetRot;
            rotate = false;
        }

        // Check ammo type and fire correct projectile
        Fire(loadout[chamberIndex]);

        // Reset ammo slot
        SetChamberLoad(chamberIndex, AmmoType.Empty);
        chamberIndex++;

        // Check if empty 
        if(IsEmpty()) {
            StartReloadingAnimation();
        }
    }

    void Fire(AmmoType type) {
        print("Bäng");
        // Todo: Raycast, choose corresponding sound effect 
    }

    void SetUICylinderTargetRotation(bool reset) {

        originalRot = crosshair.rotation;
        rotateTimer = 0;
        if(reset) {
            if(isReloading) { // Resets cylinder to reload mode
                print("Ready to load");
                targetRot = Quaternion.Euler(0, 0, 15);
            } else { // Resets cylinder to shoot mode
                print("Ready to shoot");
                targetRot = Quaternion.Euler(0, 0, 0);
            }
        } else {
            // Rotates cylinder 120degrees after loadout or shooting
            targetRot = Quaternion.Euler(0, 0, -120) * crosshair.rotation;
        }
        rotate = true;
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
        SetUICylinderTargetRotation(false);
    }

    void StartReloadingAnimation() {
        anim.Play("Reload");
    }

    public void StartReloading() {
        
        print("Started reloading");
        for(int i = 0; i < loadout.Length; i++) {
            SetChamberLoad(i, AmmoType.Empty);
            //loadout[i] = AmmoType.Empty;
        }

        isReloading = true;
        chamberIndex = 0;
        SetUICylinderTargetRotation(true);
    }

    void StopReloading() {
        // Todo: Stop loading animation here
        isReloading = false;
        print("Stopped reloading");
        reloadComplete = true;
        chamberIndex = 0;
        SetUICylinderTargetRotation(true);
    }

    void LoadChamber(AmmoType ammo) {
        // Todo: Bullet loading animation here
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

    void Rotate() {
        rotateTimer += Time.deltaTime;
        float origT;
        float t;
        if(isReloading || reloadComplete) {
            origT = rotateTimer / reloadDuration;
            t = reloadCurve.Evaluate(origT);

        } else {
            origT = rotateTimer / shootingDuration;
            t = shootingCurve.Evaluate(origT);
        }

        crosshair.rotation = Quaternion.SlerpUnclamped(originalRot, targetRot, t);
        if(origT >= 1) {
            rotate = false;
            crosshair.rotation = targetRot;
            reloadComplete = false;
        }
    }

    private void Update() {
        // Firing input
        if(Input.GetButtonDown("Fire1")) {
            PullTrigger();
        }

        // Reload input
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

        // UICylinder rotation
        if(rotate) {
            Rotate();
        }
    }
    private void Start() {
        crosshair = GameObject.Find("Crosshair").transform;
        anim = GetComponent<Animator>();
        StartReloading();
    }
}