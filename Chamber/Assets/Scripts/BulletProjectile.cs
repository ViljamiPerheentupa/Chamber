using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {
    public Transform source;
    public float moveSpeed = 3.0f;
    public float dmg = 50.0f;
    public LayerMask layerMask = 0xff;
    public Vector3 decalSize;
    public Material decalMaterial;

    void Start() {
        Invoke("DestroyMe", 60.0f);
    }

    void Update() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, moveSpeed * Time.deltaTime, layerMask)) {
            BaseHealth health = hit.collider.GetComponent<BaseHealth>();
            if (health) {
                health.TakeDamage(dmg, source);
            }

            DecalManager dm = FindObjectOfType<DecalManager>();
            if (dm) {
                dm.NewDecal(hit.point, Quaternion.LookRotation(hit.normal), decalSize, decalMaterial, hit.collider.transform);
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
