using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardVolume : MonoBehaviour {
    public float damage = 1000.0f;
    public float timer;
    bool cooldown = false;
    float damageTime;

    private void OnTriggerStay(Collider other) {
        if (!cooldown) {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health) {
                health.TakeDamage(damage, null);
                damageTime = Time.time;
                cooldown = true;
            }
        }
    }

    private void Update() {
        if (cooldown) {
            if (Time.time - damageTime >= timer) {
                cooldown = false;
            }
        }
    }
}
