﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Gun : MonoBehaviour
{
    public enum AmmoType { Empty, Piercing, eShock, AirBlast };
    AmmoType[] loadout = new AmmoType[3];
    Animator anim;
    Camera cam;    
    public Color[] colors;

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
    public float airblastRadius = 5f;
    public float airblastForceOthers = 500;
    public float airblastUpwardForce = 0.2f;
    public float airblastForcePlayer = 10;

    public AnimationCurve reloadCurve;
    public AnimationCurve shootingCurve;

    public LayerMask layerMask;
    void PullTrigger() {

        // if reloading, stop
        if(isReloading) {
            if(IsEmpty()) {
                // Play empty shot sound
                print("Click");
                Fire(loadout[chamberIndex]);
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
        //print("Bäng");
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity,  layerMask)){
            print(hit.transform.gameObject);
            if (type == AmmoType.AirBlast) {
                var objectsHit = Physics.OverlapSphere(hit.point, airblastRadius, layerMask);
                foreach (Collider col in objectsHit) {
                    var rb = col.GetComponent<Rigidbody>();
                    if (rb != null) {
                        rb.AddExplosionForce(airblastForceOthers, hit.point, airblastRadius);
                    }
                }
                var player = GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<Rigidbody>();
                var distance = Vector3.Distance(hit.point, player.transform.position);
                if (distance < airblastRadius) {
                    player.GetComponent<PlayerMover>().airblastin = true;
                    player.GetComponent<PlayerMover>().lastInputState = PlayerState.Airborne;
                    player.velocity = new Vector3(player.velocity.x, 0, player.velocity.z);
                    player.AddForce((player.position - hit.point) * airblastForcePlayer * (airblastRadius / distance), ForceMode.VelocityChange);
                }
            }
            if (hit.transform.gameObject.tag == "Hitspot" && type == AmmoType.Piercing) {
                hit.transform.GetComponent<EnemyHitspot>().HitspotHit();
            }
            if (hit.transform.gameObject.tag == "Enemy") {
                var eb = hit.transform.GetComponent<EnemyBehaviour>();
                if(eb != null) {
                    if(type == AmmoType.eShock)
                        eb.GotStunned();
                    else if(type == AmmoType.AirBlast)
                        eb.GotBlasted();
                    else if(type == AmmoType.Piercing)
                        eb.GotDamage();
                    else
                        eb.LaughAtEmptyGun();
                }
            }
            if(hit.transform.gameObject.tag == "Interactable") {
                var ih = hit.transform.GetComponent<Trigger>();
                if (ih != null) {
                    ih.ShootTrigger(type);
                }
            }
        }
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
            chamberUI[chamber].color = colors[0];
        if(ammo == AmmoType.Piercing)
            chamberUI[chamber].color = colors[3];
        if(ammo == AmmoType.eShock)
            chamberUI[chamber].color = colors[2];
        if(ammo == AmmoType.AirBlast)
            chamberUI[chamber].color = colors[1];

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
                LoadChamber(AmmoType.Piercing);
            }
            if(Input.GetKeyDown("2")) {
                LoadChamber(AmmoType.eShock);
            }
            if(Input.GetKeyDown("3")) {
                LoadChamber(AmmoType.AirBlast);
            }
        }

        // UICylinder rotation
        if(rotate) {
            Rotate();
        }
    }
    private void Start() {
        cam = Camera.main;
        crosshair = GameObject.Find("Crosshair").transform;
        layerMask = LayerMask.GetMask(new string[] { "Environment", "Enemy" });
        anim = GetComponent<Animator>();
        StartReloading();
    }
}