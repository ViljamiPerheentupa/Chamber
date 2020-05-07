using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour {
    public float maximumHealth = 100.0f;
    public float currentHealth = 100.0f;
    public bool isDead = false;

    public virtual void TakeDamage(float dmg) {
        currentHealth = Mathf.Max(0.0f, currentHealth - dmg);
        if (currentHealth == 0.0f && !isDead) {
            isDead = true;
            Die();
        }
    }

    protected virtual void Die() {

    }
}
