using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;



public class Gun : MonoBehaviour
{
    public enum AmmoType { Empty, eShock, Magnet, Time };
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

    public bool hasTime = false;
    public bool hasMagnet = false;
    public bool hasShock = false;
    public float shootingCooldown = 0.1f;
    float shootTimer = 0;

    Animator gunAnim;
    int anim1;
    int anim2;

    public GameObject abBubblePrefab;
    public GameObject abWavePrefab;
    public GameObject bulletholeDecalPrefab;
    public GameObject bulletmarkDecalPrefab;
    public GameObject genericParticlesPrefab;
    public GameObject sparksParticlesPrefab;
    public GameObject shockParticlesPrefab;

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
        if (type == AmmoType.Magnet) {
            parameter = 0;
        }
        if (type == AmmoType.eShock) {
            parameter = 1;
        }
        if (type == AmmoType.Time) {
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
            if (type != AmmoType.Empty) {
                var gParticles = Instantiate(sparksParticlesPrefab, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
                Destroy(gParticles, 1);
                if (type == AmmoType.eShock) {
                    var sParticles = Instantiate(shockParticlesPrefab, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
                    Destroy(sParticles, 1);
                }
            }
            if (type != AmmoType.Empty && hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment")) {
                //Instantiate(bulletholeDecalPrefab, hit.point - ((hit.point - GameObject.FindGameObjectWithTag("PlayerObject").transform.position) * 0.001f), Quaternion.LookRotation(hit.normal), GameObject.Find("Decals").transform);
                if (type == AmmoType.eShock) {
                    // GameObject.Find("Decals").GetComponent<DecalManager>().NewDecal(Instantiate(bulletholeDecalPrefab, hit.point - ((hit.point - GameObject.FindGameObjectWithTag("PlayerObject").transform.position) * 0.001f), Quaternion.LookRotation(hit.normal), GameObject.Find("Decals").transform));
                    GameObject.Find("Decals").GetComponent<DecalManager>().NewDecal(Instantiate(bulletmarkDecalPrefab, hit.point - ((hit.point - GameObject.FindGameObjectWithTag("PlayerObject").transform.position) * 0.001f), Quaternion.LookRotation(hit.normal), GameObject.Find("Decals").transform));
                }
                var gParticles = Instantiate(genericParticlesPrefab, hit.point + hit.normal * 0.05f, Quaternion.LookRotation(hit.normal));
                Destroy(gParticles, 15);
            }
            /*if (type == AmmoType.AirBlast) {
                Instantiate(abBubblePrefab, hit.point, transform.rotation);
                Instantiate(abWavePrefab, hit.point, Quaternion.LookRotation(hit.normal));
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
            }*/
            /*if (hit.transform.gameObject.tag == "Hitspot" && type == AmmoType.Piercing) {
            }*/
            if (hit.transform.gameObject.tag == "Hitspot" && type == AmmoType.eShock) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitBullet", hit.point);
                hit.transform.GetComponent<EnemyHitspot>().HitspotHit();
                // var eb = hit.transform.GetComponentInParent<EnemyBehaviour>();
                // eb.GotStunned();
            }
            if (hit.transform.gameObject.tag == "Enemy") {
                var eb = hit.transform.GetComponent<EnemyBehaviour>();
                if(eb != null) {
                    if(type == AmmoType.eShock)
                        eb.GotStunned();
                    else if(type == AmmoType.Time)
                        eb.GotBlasted();
                    else if(type == AmmoType.Magnet) {
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
            if (hit.transform.gameObject.tag == "Enemy" && type != AmmoType.Empty) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/MissEnemy", hit.point);
            }
            else if(type == AmmoType.Time) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitGeneric", hit.point);
            }
            if (type == AmmoType.eShock) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", hit.point);
            }
            if(type == AmmoType.Magnet) {
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
        
        shootTimer = Time.time + shootingCooldown;
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
        if(ammo == AmmoType.eShock)
            chamberUI[chamber].color = colors[1];
        if(ammo == AmmoType.Magnet)
            chamberUI[chamber].color = colors[2];
        if(ammo == AmmoType.Time)
            chamberUI[chamber].color = colors[3];

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
        // Don't fire if cooling down
        if (Time.time <= shootTimer) {
            return;
        }

        // Firing input
        if(Input.GetButton("Fire1")) {
            // Check that we're not reloading, and that the chamber is valid (just in case)
            if (!isReloading && chamberIndex >= 0 && chamberIndex < 3) {
                FireAnimation(loadout[chamberIndex]);
            }
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

        if (isReloading) {
            gunAnim.SetBool("inReload", true);
            if (hasShock) {
                if (Input.GetKey("1")) {
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
                if (Input.GetKeyUp("1")) {
                    gunAnim.Play("gun_reloadinsert");
                    LoadChamber(AmmoType.eShock);
                    fillTimer = 0;
                }
            }
            if (hasTime) {
                if (Input.GetKey("3")) {
                    fillTimer += Time.deltaTime;
                    if (fillTimer >= fillDuration) {
                        for (int i = 0; i < 3 - chamberIndex; i++) {
                            SetChamberLoad(chamberIndex, AmmoType.Time);
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
                    LoadChamber(AmmoType.Time);
                    fillTimer = 0;
                }
            }
            if (hasMagnet) {
                if (Input.GetKey("2")) {
                    fillTimer += Time.deltaTime;
                    if (fillTimer >= fillDuration) {
                        for (int i = 0; i < 3 - chamberIndex; i++) {
                            SetChamberLoad(chamberIndex, AmmoType.Magnet);
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
                    LoadChamber(AmmoType.Magnet);
                    fillTimer = 0;
                }
            }
        } else gunAnim.SetBool("inReload", false);

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

        // Set the crosshair images to the default color
        for (int i = 0; i < 3; i++) {
            chamberUI[i].color = colors[0];
        }
    }
}