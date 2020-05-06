using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class GunContainer : MonoBehaviour {
    // Constants
    private const int numTypes = 3;
    public enum AmmoType { Empty, eShock, Magnet, Time };

    // Public
    public DecalManager decalManager;
    public RectTransform crosshair;
    public Image[] reticleImage = new Image[3];
    public GunAmmoBase[] ammoTypes = new GunAmmoBase[numTypes];
    public bool[] enabledAmmoTypes = new bool[numTypes];
    public float fireCooldownTime = 0.3f;
    public Color emptyColor;
    public float chamberUiRotateDuration = 0.5f;
    public Animator animator;

    // Private
    private bool isHolstering = false;
    private uint rotationPrevSlot = 3;
    private float targetAngle = 0.0f;
    private float startAngle = 0.0f;
    private float startRotateTime = 0.0f;
    private bool isInReload = false;
    private float nextFire = 0.0f;
    private Camera cam;
    private AmmoType[] chambers = new AmmoType[3];
    private uint currentChamber = 0;
    private bool isHoldMode = false;

    void Start() {
        cam = Camera.main;

        for (int i = 0; i < 3; ++i) {
            chambers[i] = AmmoType.Empty;
            reticleImage[i].color = emptyColor;
        }
    }

    void AddAmmoToChamber(AmmoType type) {
        animator.Play("gun_reloadinsert");

        // Set Crosshair Color
        int itype = ((int)type) - 1;
        reticleImage[currentChamber].color = ammoTypes[itype].color;

        // Set Chamber Bullet
        chambers[currentChamber] = type;

        // Close Reload Mode if all are filled
        if (++currentChamber == 3) {
            isInReload = false;
            currentChamber = 0;
            animator.Play("gun_endreload");
        }
        
        SetTargetAngle(currentChamber);
    }

    public void SetHolstering(bool status) {
        isHolstering = status;

        if (isInReload) {
            isInReload = false;
            currentChamber = 0;
            animator.Play("gun_endreload");
            SetTargetAngle(0);
        }

        // Do animations
    }

    void Update() {
        // Calculate crosshair position
        float ang = (Time.time - startRotateTime) / chamberUiRotateDuration;
        ang = Mathf.Clamp(ang, 0.0f, 1.0f);
        ang = Mathf.Lerp(startAngle, targetAngle, ang);
        SetCrosshairToAngle(ang);

        if (isHolstering) {
            return;
        }

        // In cooldown
        if (nextFire > Time.time) {
            return;
        }


        if (isInReload) {
            if (Input.GetButtonDown("Reload") || Input.GetButtonDown("Fire1")) {
                isInReload = false;
                currentChamber = 0;
                animator.Play("gun_endreload");
                SetTargetAngle(0);
            }
            else if (Input.GetButtonDown("AmmoSlot1") && enabledAmmoTypes[0]) {
                AddAmmoToChamber(AmmoType.eShock);
            }
            else if (Input.GetButtonDown("AmmoSlot2") && enabledAmmoTypes[1]) {
                AddAmmoToChamber(AmmoType.Magnet);
            }
            else if (Input.GetButtonDown("AmmoSlot3") && enabledAmmoTypes[2]) {
                AddAmmoToChamber(AmmoType.Time);
            }
        }
        else {
            if (Input.GetButtonDown("Reload")) {
                isInReload = true;
                currentChamber = 0;
                animator.SetTrigger("Reloading");
                SetTargetAngle(0);

                // Empty Chambers
                for (int i = 0; i < 3; ++i) {
                    chambers[i] = AmmoType.Empty;
                    reticleImage[i].color = emptyColor;
                }
            }

            
            
            if (chambers[currentChamber] == AmmoType.Empty) {
                if (Input.GetButtonDown("Fire1")) {
                    // Empty Fire
                    animator.Play("shoot_empty");

                    // Next Chamber
                    SwapToNextChamber();
                    WaitForNextShot();
                }
            }
            else {
                // Possibility of firing a round...

                // Getting transform data
                Vector3 pos = cam.transform.position;
                Vector3 fwd = cam.transform.forward;

                // What type of ammo is it?
                int currentChamberType = (int)chambers[currentChamber] - 1;

                // If starting to fire
                if (Input.GetButtonDown("Fire1")) {
                    ammoTypes[currentChamberType].OnFire(pos, fwd);
                }
                else if (isHoldMode) {
                    if (Input.GetButton("Fire1")) {
                        ammoTypes[currentChamberType].OnFireHold(pos, fwd);
                    }
                    else if (Input.GetButtonUp("Fire1")) {
                        ammoTypes[currentChamberType].OnFireRelease(pos, fwd);
                    }
                }
            }
        }
    }

    private void SetTargetAngle(uint targetSlot) {
        // If we're not at angle zero and we want to be, instead go all the way around.
        if (targetSlot == 0)
            targetSlot = 3;

        if (rotationPrevSlot == targetSlot) {
            return;
        }

        rotationPrevSlot = targetSlot;

        targetAngle = targetSlot * 360.0f / 3.0f;
        startAngle = crosshair.eulerAngles.z;
        startRotateTime = Time.time;
    }

    private void SetCrosshairToAngle(float angle) {
        crosshair.eulerAngles = new Vector3(0, 0, angle);
    }

    // ===========================
    // Bullet Interfaces
    // ===========================
    public void PlayFireAnimation() {
        int i = Random.Range(1, 4);
        animator.Play("shoot_fire" + i);
    }

    public void CreateDecal(GameObject decal, Vector3 pos, Vector3 normal) {
        decalManager.NewDecal(Instantiate(decal, pos, Quaternion.LookRotation(normal), decalManager.transform));
    }

    public void SetHoldMode(bool holdMode) {
        isHoldMode = holdMode;
    }

    public void SetCurrentChamberColor(Color color) {
        reticleImage[currentChamber].color = color;
    }

    public void SetCurrentChamber(AmmoType type) {
        chambers[currentChamber] = type;
        reticleImage[currentChamber].color = emptyColor;
    }

    public void WaitForNextShot() {
        nextFire = Time.time + fireCooldownTime;
    }

    public void SwapToNextChamber() {
        currentChamber = (currentChamber + 1) % 3;
        SetTargetAngle(currentChamber);
    }

    public void SetAmmoTypeStatus(AmmoType type, bool status) {
        int itype = ((int)type) - 1;

        if (itype >= 0 && itype < numTypes)
            enabledAmmoTypes[itype] = status;
    }
}
 