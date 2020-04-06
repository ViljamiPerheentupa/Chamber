using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitspot : MonoBehaviour
{
    public GameObject particlesPrefab;
    public void HitspotHit() {
        gameObject.transform.parent.GetComponent<EnemyHealthManager>().hitspots.Remove(gameObject);
        if (particlesPrefab != null) {
            var particles = Instantiate(particlesPrefab, transform.position, transform.rotation);
            Destroy(particles, 5);
        }
        GetComponent<EnemyBehaviour>().Death();
        Destroy(gameObject);
    }
}
