using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShotgun : MonoBehaviour {
    public bool isActivated = true;
    public LayerMask targetHitLayers;
    public float forceAmount = 10.0f;
    public float forceRadius = 1.0f;
    public float selfForceAmount = 20.0f;
    public float fireCooldownTime = 0.3f;
    private float nextFire = 0.0f;
    Rigidbody rb;
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        GameManager gm = FindObjectOfType<GameManager>();
        if ((gm && gm.paused) || GetComponent<PlayerHealth>().isDead || !isActivated) {
            return;
        }
        
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
                float speed = Mathf.Max(rb.velocity.magnitude, selfForceAmount);
                rb.velocity = -fwd * speed;
            }
            
            nextFire = Time.time + fireCooldownTime;
        }
    }
}
