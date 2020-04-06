using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;



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

    public float fillDuration = 0.15f;
    float fillTimer = 0;
    public bool holdToStartReload = true;
    public float loadDuration = 0.2f;
    float loadTimer = 0;
    public bool inputSpamEnabled = true;
    public int inputSpamLimit = 3;
    int inputSpam = 0;
    public float inputSpamDuration = 0.3f;
    float inputSpamTimer = 0;

    public bool hasNormal = false;
    public bool hasAirburst = false;
    public bool hasShock = false;
    public float shootingCooldown = 0.3f;
    bool shootCD = false;
    float shootTimer = 0;

    Animator gunAnim;
    int anim1;
    int anim2;

    public void PullTrigger() {

        // if reloading, stop
        if(isReloading) {
            if(IsEmpty()) {
                // Play empty shot sound
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
        float parameter = 0;
        if (type == AmmoType.Piercing) {
            parameter = 0;
        }
        if (type == AmmoType.eShock) {
            parameter = 1;
        }
        if (type == AmmoType.AirBlast) {
            parameter = 2;
        }
        if (type == AmmoType.Empty) {
            parameter = -1;
        }

        if (parameter != -1) {
            FMOD.Studio.EventInstance gunSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Gunshot");
            gunSound.setParameterByName("AmmoType", parameter);
            gunSound.start();
            gunSound.release();
        }


        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity,  layerMask)){
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitBullet", hit.point);
                hit.transform.GetComponent<EnemyHitspot>().HitspotHit();
            }
            if (hit.transform.gameObject.tag == "Hitspot" && type == AmmoType.eShock) {
                var eb = hit.transform.GetComponentInParent<EnemyBehaviour>();
                eb.GotStunned();
            }
            if (hit.transform.gameObject.tag == "Enemy") {
                var eb = hit.transform.GetComponent<EnemyBehaviour>();
                if(eb != null) {
                    if(type == AmmoType.eShock)
                        eb.GotStunned();
                    else if(type == AmmoType.AirBlast)
                        eb.GotBlasted();
                    else if(type == AmmoType.Piercing) {
                        print("missed vitals");
                    }


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
            // Hit SFX
            if (hit.transform.gameObject.tag == "Enemy") {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MissEnemy", hit.point);
            }
            else if(type == AmmoType.Piercing) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitGeneric", hit.point);
            }
            if (type == AmmoType.eShock) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", hit.point);
            }
            if(type == AmmoType.AirBlast) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitAir", hit.point);
            }

        }

    }

    void FireAnimation(AmmoType type) {
        if (type != AmmoType.Empty) {
            int i = Random.Range(1, 4);
            if (anim2 != 0) {
                if (i == anim2) {
                    while (i == anim2) {
                        i = Random.Range(1, 4);
                    }
                    anim1 = 0;
                    anim2 = 0;
                }
            }
            if (anim1 != 0) {
                if (i == anim1) {
                    anim2 = i;
                }
            }
            anim1 = i;
            gunAnim.Play("shoot_fire" + i);
        } else gunAnim.Play("shoot_empty");
        shootCD = true;
    }

    void SetUICylinderTargetRotation(bool reset) {

        originalRot = crosshair.rotation;
        rotateTimer = 0;
        if(reset) {
            if(isReloading) { // Resets cylinder to reload mode
                targetRot = Quaternion.Euler(0, 0, 15);
            } else { // Resets cylinder to shoot mode
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
        for (int i = 0; i < loadout.Length; i++) {
            SetChamberLoad(i, AmmoType.Empty);
        }
        gunAnim.SetTrigger("Reloading");
    }

    public void StartReloading() {
        inputSpam = 0;
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
        gunAnim.Play("gun_endreload");
        isReloading = false;
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
        if (shootCD) {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootingCooldown) {
                shootCD = false;
                shootTimer = 0;
            }
        }

        // Firing input
        if(Input.GetButton("Fire1") && !shootCD) {
            if (isReloading && chamberIndex < 0) {
                FireAnimation(loadout[chamberIndex-1]);
            } else FireAnimation(loadout[chamberIndex]);
        }

        // Reload input
        if(holdToStartReload && !isReloading && (Input.GetKey("1") || Input.GetKey("2") || Input.GetKey("3"))){
            loadTimer += Time.deltaTime;
            if (loadTimer >= loadDuration) {
                loadTimer = 0;
                StartReloadingAnimation();
            }
        }
        if (inputSpamEnabled && !isReloading) {
            if (Input.GetKeyDown("1") || Input.GetKeyDown("2") || Input.GetKeyDown("3")) {
                inputSpam++;
                inputSpamTimer = 0;
            }
            if (inputSpam > 0) {
                inputSpamTimer += Time.deltaTime;
                if (inputSpamTimer >= inputSpamDuration) {
                    inputSpam = 0;
                    inputSpamTimer = 0;
                }
            }
            if (inputSpam >= inputSpamLimit) {
                StartReloadingAnimation();
            }
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            if(!isReloading)
                StartReloadingAnimation();
            else
                StopReloading();
        }

        if(isReloading) {
            if (hasNormal) {
                if (Input.GetKey("1")) {
                    fillTimer += Time.deltaTime;
                    if (fillTimer >= fillDuration) {
                        for (int i = 0; i < 3 - chamberIndex; i++) {
                            SetChamberLoad(chamberIndex, AmmoType.Piercing);
                            chamberIndex++;
                            if (chamberIndex > 2) {
                                fillTimer = 0;
                                StopReloading();
                                return;
                            }
                        }
                    }
                }
                if (Input.GetKeyUp("1")) {
                    gunAnim.Play("gun_reloadinsert");
                    LoadChamber(AmmoType.Piercing);
                    fillTimer = 0;
                }
            }
            if (hasShock) {
                if (Input.GetKey("3")) {
                    fillTimer += Time.deltaTime;
                    if (fillTimer >= fillDuration) {
                        for (int i = 0; i < 3 - chamberIndex; i++) {
                            SetChamberLoad(chamberIndex, AmmoType.eShock);
                            chamberIndex++;
                            if (chamberIndex > 2) {
                                fillTimer = 0;
                                StopReloading();
                                return;
                            }
                        }
                    }
                }
                if (Input.GetKeyUp("3")) {
                    gunAnim.Play("gun_reloadinsert");
                    LoadChamber(AmmoType.eShock);
                    fillTimer = 0;
                }
            }
            if (hasAirburst) {
                if (Input.GetKey("2")) {
                    fillTimer += Time.deltaTime;
                    if (fillTimer >= fillDuration) {
                        for (int i = 0; i < 3 - chamberIndex; i++) {
                            SetChamberLoad(chamberIndex, AmmoType.AirBlast);
                            chamberIndex++;
                            if (chamberIndex > 2) {
                                fillTimer = 0;
                                StopReloading();
                                return;
                            }
                        }
                    }
                }
                if (Input.GetKeyUp("2")) {
                    gunAnim.Play("gun_reloadinsert");
                    LoadChamber(AmmoType.AirBlast);
                    fillTimer = 0;
                }
            }
        }

        // UICylinder rotation
        if(rotate) {
            Rotate();
        }
    }
    private void Start() {
        cam = Camera.main;
        gunAnim = GameObject.FindGameObjectWithTag("GunRender").GetComponent<Animator>();
        crosshair = GameObject.Find("Crosshair").transform;
        //layerMask = LayerMask.GetMask(new string[] { "Environment", "Enemy", "Props" });
        anim = GetComponent<Animator>();
        //StartReloading();
    }
}