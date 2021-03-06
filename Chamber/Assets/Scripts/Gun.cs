using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using FMODUnity;

public class Gun : MonoBehaviour {
    // Constants
    private const int numTypes = 3;
    public enum AmmoType { Empty, eShock, Switcheroo, Time };

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
    public Transform muzzleTransform;
    public Color[] lineRendererColors = new Color[3];
    public float tracerFadeDuration = 0.8f;

    // Private
    private LineRenderer lineRenderer;
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
    private bool isHoldingFire = false;
    private float startTracerTime;
    private Color currentTracerColor;

    void Start() {
        cam = Camera.main;

        lineRenderer = GetComponent<LineRenderer>();

        for (int i = 0; i < 3; ++i) {
            chambers[i] = AmmoType.Empty;
            reticleImage[i].color = emptyColor;
            ammoTypes[i].SetGun(this);
        }
    }

    public void StartReset() {
        isHolstering = false;
        rotationPrevSlot = 3;
        targetAngle = 0.0f;
        startAngle = 0.0f;
        startRotateTime = 0.0f;
        nextFire = 0.0f;
        isHoldMode = false;
        currentChamber = 0;
        if (isInReload) {
            isInReload = false;
            animator.Play("gun_endreload");
        }

        for (int i = 0; i < 3; ++i) {
            chambers[i] = AmmoType.Empty;
            reticleImage[i].color = emptyColor;
        }
        
        animator.gameObject.SetActive(true);
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
        animator.gameObject.SetActive(!status);
    }

    void ExitReload() {
        isInReload = false;
        currentChamber = 0;
        animator.Play("gun_endreload");
        SetTargetAngle(0);
    }

    void OnFire(InputValue value) {

        isHoldingFire = value.isPressed;

        if (isInReload) {
            ExitReload();
        }
        else {
            if (chambers[currentChamber] == AmmoType.Empty) {
                if (canDoActionAndNoHolster()) {
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
                if (value.isPressed && canDoActionAndNoHolster()) {
                    ammoTypes[currentChamberType].FirePress(pos, fwd);
                }
                else if (isHoldMode) {
                    ammoTypes[currentChamberType].FireRelease(pos, fwd);
                }
            }
        }
    }

    bool canDoAction() {
        return (!GameManager.Instance.isPaused && !GetComponent<PlayerHealth>().isDead && (nextFire < Time.time));
    }

    bool canDoActionAndNoHolster() {
        return (canDoAction() && !isHolstering);
    }

    void OnReload() {
        if (isHoldingFire || !canDoActionAndNoHolster()) return;

        if (isInReload) {
            ExitReload();
        }
        else {
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
    }

    void OnAmmoShock() {
        if (isInReload && enabledAmmoTypes[0] && canDoActionAndNoHolster()) {
            AddAmmoToChamber(AmmoType.eShock);
        }
    }

    void OnAmmoMagnetise() {
        if (isInReload && enabledAmmoTypes[1] && canDoActionAndNoHolster()) {
            AddAmmoToChamber(AmmoType.Switcheroo);
        }
    }

    void OnAmmoTime() {
        if (isInReload && enabledAmmoTypes[2] && canDoActionAndNoHolster()) {
            AddAmmoToChamber(AmmoType.Time);
        }
    }

    void Update() {
        // Calculate crosshair position
        float ang = (Time.time - startRotateTime) / chamberUiRotateDuration;
        ang = Mathf.Clamp(ang, 0.0f, 1.0f);
        ang = Mathf.Lerp(startAngle, targetAngle, ang);
        SetCrosshairToAngle(ang);

        // Handle Tracer
        float a = 1f - (Time.time - startTracerTime) / tracerFadeDuration;
        lineRenderer.material.SetColor("_TintColor", new Color(currentTracerColor.r, currentTracerColor.g, currentTracerColor.b, a));
    
        if (chambers[currentChamber] != AmmoType.Empty) {
            int currentChamberType = (int)chambers[currentChamber] - 1;
            ammoTypes[currentChamberType].FireHold(transform.position, transform.forward);
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

    public void CreateDecal(Vector3 position, Quaternion rotation, Vector3 size, Material material, Transform parent) {
        decalManager.NewDecal(position, rotation, size, material, parent);
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

    public void FireLineRenderer(Vector3 targetPos, int i) {
        lineRenderer.enabled = true;
        currentTracerColor = lineRendererColors[i];
        lineRenderer.SetPosition(0, muzzleTransform.position);
        lineRenderer.SetPosition(1, targetPos);

        startTracerTime = Time.time;
        Invoke("RemoveLine", tracerFadeDuration);
    }

    void RemoveLine() {
        lineRenderer.enabled = false;
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
 