using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardVolume : MonoBehaviour {
    public float damage = 1000.0f;

    private void OnTriggerEnter(Collider other) {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health) {
            health.TakeDamage(damage);
        }
    }
}
