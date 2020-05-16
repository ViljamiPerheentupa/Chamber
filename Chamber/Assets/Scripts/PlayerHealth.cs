using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : BaseHealth {
    public float timeToRecover = 2.0f;
    public float healthRecoverPerSec = 5.0f;
    public AnimationCurve recoverAnimation = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    public Transform blackOut;
    public float fadeToBlackTime = 2.0f;
    public float blackToWhiteTime = 0.5f;
    public float fadeFromWhiteTime = 1.0f;
    public float damageIndicatorFadeOutTime = 0.5f;
    public float targetFov = 140.0f;
    public AnimationCurve fovCurve;

    private float startHealTime = 0.0f;
    private float fadeTime;
    private float originalFov;
    private List<DamageIndicatorFadeOut> damageIndicators = new List<DamageIndicatorFadeOut>();
    public Transform canvas;
    public Transform damagePrefab;

    public override void TakeDamage(float dmg, Transform source) {
        base.TakeDamage(dmg, source);
        startHealTime = Time.time + timeToRecover;

        if (source) {
            foreach (DamageIndicatorFadeOut indicator in damageIndicators) {
                if (indicator.source == source) {
                    indicator.lastHitTime = Time.time;
                    return;
                }
            }

            RectTransform d = Instantiate(damagePrefab, new Vector3(0,0,0), new Quaternion(), canvas) as RectTransform;
            d.anchoredPosition = new Vector2(0,0);
            Vector2 fwd = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 dir = new Vector2(source.position.x - transform.position.x, source.position.z - transform.position.z);
            d.eulerAngles = new Vector3(0.0f, 0.0f, Vector2.SignedAngle(fwd, dir));
            DamageIndicatorFadeOut newIndicator = d.GetComponent<DamageIndicatorFadeOut>();
            newIndicator.lastHitTime = Time.time;
            newIndicator.source = source;
            damageIndicators.Add(newIndicator);
        }
    }

    protected override void Die() {
        GetComponent<Rigidbody>().velocity = new Vector3();

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn() {
        fadeTime = Time.time + fadeToBlackTime;
        originalFov = Camera.main.fieldOfView;

        while (Time.time < fadeTime) {
            float t = 1.0f - (fadeTime - Time.time) / fadeToBlackTime;
            blackOut.GetComponent<Image>().color = new Color(0, 0, 0, t);
            t = fovCurve.Evaluate(t);
            Camera.main.fieldOfView = Mathf.Lerp(originalFov, targetFov, t);
            yield return null;
        }
        
        CheckpointManager cm = GameObject.FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.StartReset();
        }

        fadeTime = Time.time + blackToWhiteTime;
        while (Time.time < fadeTime) {
            float t = 1.0f - ((fadeTime - Time.time) / blackToWhiteTime);
            blackOut.GetComponent<Image>().color = new Color(t, t, t, 1.0f);
            yield return null;
        }

        fadeTime = Time.time + fadeFromWhiteTime;
        while (Time.time < fadeTime) {
            float t = (fadeTime - Time.time) / fadeFromWhiteTime;
            blackOut.GetComponent<Image>().color = new Color(1f, 1f, 1f, t);
            t = fovCurve.Evaluate(t);
            Camera.main.fieldOfView = Mathf.Lerp(originalFov, targetFov, t);
            yield return null;
        }
        
        blackOut.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        Camera.main.fieldOfView = originalFov;
    }

    void Update() {
        if (!isDead) {
            if (Time.time > startHealTime) {
                currentHealth = Mathf.Min(maximumHealth, currentHealth + healthRecoverPerSec * Time.deltaTime * recoverAnimation.Evaluate(Time.time - startHealTime));
            }
        }

        Vector2 fwd = new Vector2(transform.forward.x, transform.forward.z);
        for (int i = damageIndicators.Count - 1; i >= 0; --i) {
            DamageIndicatorFadeOut indicator = damageIndicators[i];

            if (indicator.lastHitTime + damageIndicatorFadeOutTime < Time.time) {
                damageIndicators.RemoveAt(i);
                Destroy(indicator.gameObject);
            }
            else {
                Vector2 dir = new Vector2(indicator.source.position.x - transform.position.x, indicator.source.position.z - transform.position.z);
                ((RectTransform)indicator.transform).eulerAngles = new Vector3(0.0f, 0.0f, Vector2.SignedAngle(fwd, dir));
                indicator.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f - (Time.time - indicator.lastHitTime) / damageIndicatorFadeOutTime);
            }
        }
    }
}
