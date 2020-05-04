using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirShotgun : MonoBehaviour {
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
        // In cooldown
        if (nextFire > Time.time) {
            return;
        }

        Vector3 pos = Camera.main.transform.position;
        Vector3 fwd = Camera.main.transform.forward;
        if (Input.GetButtonDown("Fire2")) {
            Debug.Log("Airshotgunn Fired!");
            Collider[] hitColliders = Physics.OverlapSphere(pos + fwd * forceRadius, forceRadius - 0.1f, targetHitLayers);
            for (int i = 0; i < hitColliders.Length; ++i) {
                Debug.Log(hitColliders[i].gameObject.name);
                if (hitColliders[i].attachedRigidbody) {
                    Vector3 forceDir = hitColliders[i].transform.position - pos;
                    hitColliders[i].attachedRigidbody.AddForce(forceDir * forceAmount, ForceMode.Impulse);
                }
            }

            float d = Vector3.Dot(-Vector3.up, fwd);
            if (d > 0.5) {
                if (rb) {
                    rb.AddForce(-fwd * selfForceAmount * (d - 0.5f) * 2.0f, ForceMode.Impulse);
                }
            }
            
            nextFire = Time.time + fireCooldownTime;
        }
    }
}
