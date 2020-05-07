using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : BaseResetable {
    public float maximumHealth = 100.0f;
    public float currentHealth = 100.0f;
    public bool isDead = false;

    private float startHealth;

    void Start() {
        startHealth = currentHealth;
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public override void StartReset() {
        currentHealth = startHealth;
        isDead = (currentHealth == 0.0f);
    }

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
