using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitspot : MonoBehaviour
{
    public GameObject particlesPrefab;
    public void HitspotHit() {
        GetComponentInParent<EnemyHealthManager>().hitspots.Remove(gameObject);
        if (particlesPrefab != null) {
            var particles = Instantiate(particlesPrefab, transform.position, transform.rotation);
            Destroy(particles, 5);
        }
        Destroy(gameObject);
    }
}
