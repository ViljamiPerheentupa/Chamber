using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour, IPlayerDamage
{
    public int health;
    public int maxHealth = 100;
    float regenTimer = 0;
    public float regenDelay = 2f;
    bool tookDamage = false;
    public int regenPerTick = 1;
    float healTimer = 0;
    float healTicker = 0.1f;

    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < maxHealth) {
            regenTimer += Time.deltaTime;
            if (tookDamage) {
                regenTimer = 0;
                tookDamage = false;
                return;
            }
            if (regenTimer >= regenDelay) {
                healTimer += Time.deltaTime;
                while (healTimer >= healTicker) {
                    healTimer -= healTicker;
                    health += regenPerTick;
                }
                if (health >= maxHealth) {
                    health = maxHealth;
                    healTimer = 0;
                    regenTimer = 0;
                }
            }
        }
    }



    public void TakeDamage(int damage) {
        health -= damage;
        print("TOOK DAMAGE! Damage Taken: " + damage + " Health Left: " + health);
        tookDamage = true;
    }
}
