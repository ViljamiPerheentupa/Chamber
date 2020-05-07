using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth {
    public float timeToRecover = 2.0f;
    public float healthRecoverPerSec = 5.0f;
    public AnimationCurve recoverAnimation = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    
    private float startHealTime = 0.0f;

    public override void TakeDamage(float dmg) {
        base.TakeDamage(dmg);
        Debug.Log("Damage: " + dmg + ", Health: " + currentHealth);
        startHealTime = Time.time + timeToRecover;
    }

    protected override void Die() {
        Debug.Log("Dead");
    }

    void Update() {
        if (Time.time > startHealTime) {
            currentHealth = Mathf.Min(maximumHealth, currentHealth + healthRecoverPerSec * Time.deltaTime * recoverAnimation.Evaluate(Time.time - startHealTime));
        }
    }
}
