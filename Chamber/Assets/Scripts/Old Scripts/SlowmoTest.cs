
#pragma warning disable CS0414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowmoTest : MonoBehaviour
{
    public float peakSlowmo = 0.80f;
    public float maxDuration = 5f;
    float amount;
    bool cooldown;
    public Image uiElement;
    public Color depletedColor;
    Color normalColor;
    bool flashing = false;
    public AnimationCurve flashCurve;
    float flashTimer = 0;

    FMOD.Studio.EventInstance slowmo;

    private void Start() {
        normalColor = uiElement.color;
        amount = maxDuration;
        slowmo = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/BulletTime");
        slowmo.setParameterByName("BulletTime", 0);
        slowmo.start();
        slowmo.release();
    }
    void Update()
    {
        if (!GameManager.Instance.isPaused) {
            if (Input.GetButton("Fire2") && !cooldown) {
                Time.timeScale = peakSlowmo;
                slowmo.setParameterByName("BulletTime", 1);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                amount -= Time.unscaledDeltaTime;
                if (amount <= 0) {
                    cooldown = true;
                    amount = 0;
                }
            } else {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
                slowmo.setParameterByName("BulletTime", 0);
                if (amount < maxDuration) {
                    amount += Time.unscaledDeltaTime / 4;
                    if (amount > maxDuration) {
                        amount = maxDuration;
                    }
                }
            }
            if (cooldown) {
                FlashBar();
                if (amount >= maxDuration) {
                    cooldown = false;
                    flashing = false;
                }
            } else if (!cooldown) uiElement.color = normalColor;
            uiElement.fillAmount = amount / maxDuration;
        }
    }

    void FlashBar() {
        flashing = true;
        flashTimer += Time.deltaTime;
        float t = flashCurve.Evaluate(flashTimer);
        uiElement.color = new Color(t, 0, 0);
    }

    private void OnDestroy() {
        slowmo.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
