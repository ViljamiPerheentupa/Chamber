using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShotgun : MonoBehaviour {
    public bool isActivated = true;
    public LayerMask targetHitLayers;
    public float forceAmount = 10.0f;
    public float forceRadius = 1.0f;
    public float fireCooldownTime = 0.3f;
    [Tooltip("Player velocity at non-downward angles.")]
    public float selfForceAmount = 20.0f;
    [Tooltip("Player velocity at downward angles.")]
    public float downwardSelfForceAmount = 10.0f;
    [Tooltip("cos(angle) by which to lerp the player velocity values.")]
    [Range(0f, 1f)]
    public float downwardAngleMin = 0.5f;
    [Tooltip("cos(angle) by which to lerp the player velocity values.")]
    [Range(0f, 1f)]
    public float downwardAngleMax = 0.8f;
    public GameObject airShotgunUiParent;

    RectTransform uiTransform;
    float uiMaxWidth = 0.0f;
    float nextFire = 0.0f;
    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        uiTransform = (RectTransform)airShotgunUiParent.transform.GetChild(0).transform;
        uiMaxWidth = uiTransform.sizeDelta.x;
        airShotgunUiParent.SetActive(isActivated);
    }

    public void SetStatus(bool status) {
        airShotgunUiParent.SetActive(status);
        isActivated = status;
    }

    void Update() {
        GameManager gm = FindObjectOfType<GameManager>();
        if ((gm && gm.paused) || GetComponent<PlayerHealth>().isDead || !isActivated) {
            return;
        }

        float wAmt = 1f - (nextFire - Time.time) / fireCooldownTime;
        wAmt = Mathf.Clamp(wAmt, 0f, 1f);
        uiTransform.sizeDelta = new Vector2(wAmt * uiMaxWidth, uiTransform.sizeDelta.y);
        
        // In cooldown
        if (nextFire > Time.time) {
            return;
        }

        Vector3 pos = Camera.main.transform.position;
        Vector3 fwd = Camera.main.transform.forward;
        if (Input.GetButtonDown("Fire2")) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitAir", transform.position);
            
            Collider[] hitColliders = Physics.OverlapSphere(pos + fwd * forceRadius, forceRadius - 0.1f, targetHitLayers);
            for (int i = 0; i < hitColliders.Length; ++i) {
                if (hitColliders[i].GetComponent<IProp>() != null) {
                    Vector3 forceDir = hitColliders[i].transform.position - pos;
                    hitColliders[i].GetComponent<IProp>().PropForce(forceDir * forceAmount, ForceMode.Impulse);
                }
            }

            if (rb) {
                float dt = Vector3.Dot(-fwd, Vector3.up);
                float d = Mathf.Clamp((dt - downwardAngleMin) / downwardAngleMax, 0f, 1f);
                float speed = Mathf.Lerp(selfForceAmount, downwardSelfForceAmount, d);
                speed = Mathf.Max(rb.velocity.magnitude, speed);
                rb.velocity = -fwd * speed;
            }
            
            nextFire = Time.time + fireCooldownTime;
        }
    }
}
