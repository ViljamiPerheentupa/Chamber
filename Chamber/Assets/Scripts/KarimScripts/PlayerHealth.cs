using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : BaseHealth {
    public float timeToRecover = 2.0f;
    public float healthRecoverPerSec = 5.0f;
    public AnimationCurve recoverAnimation = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public Transform blackOut;
    public float fadeToBlackTime = 2.0f;
    public float blackToWhiteTime = 0.5f;
    public float fadeFromWhiteTime = 1.0f;

    private float startHealTime = 0.0f;
    private float fadeTime;
    

    public override void TakeDamage(float dmg) {
        base.TakeDamage(dmg);
        startHealTime = Time.time + timeToRecover;
    }

    protected override void Die() {
        GetComponent<Rigidbody>().velocity = new Vector3();

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn() {
        fadeTime = Time.time + fadeToBlackTime;
        while (Time.time < fadeTime) {
            blackOut.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1.0f - ((fadeTime - Time.time) / fadeToBlackTime));
            yield return null;
        }
        
        CheckpointManager cm = GameObject.FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.StartReset();
        }

        fadeTime = Time.time + blackToWhiteTime;
        while (Time.time < fadeTime) {
            float t = 1.0f - ((fadeTime - Time.time) / blackToWhiteTime);
            blackOut.GetComponent<Renderer>().material.color = new Color(t, t, t, 1.0f);
            yield return null;
        }

        fadeTime = Time.time + fadeFromWhiteTime;
        while (Time.time < fadeTime) {
            blackOut.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, (fadeTime - Time.time) / fadeFromWhiteTime);
            yield return null;
        }
    }

    void Update() {
        if (Time.time > startHealTime) {
            currentHealth = Mathf.Min(maximumHealth, currentHealth + healthRecoverPerSec * Time.deltaTime * recoverAnimation.Evaluate(Time.time - startHealTime));
        }
        
        if (Input.GetKeyDown("p")) {
            TakeDamage(maximumHealth);
        }
    }
}
