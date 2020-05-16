using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {
    public Transform source;
    public float moveSpeed = 3.0f;
    public float dmg = 50.0f;
    public LayerMask layerMask = 0xff;

    void Start() {
        Invoke("DestroyMe", 20.0f);
    }

    void Update() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, moveSpeed * Time.deltaTime, layerMask)) {
            BaseHealth health = hit.collider.GetComponent<BaseHealth>();
            if (health) {
                health.TakeDamage(dmg, source);
            }

            DestroyMe();
        }
        else {
            transform.position -= transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    void DestroyMe() {
        Destroy(gameObject);
    }
}
