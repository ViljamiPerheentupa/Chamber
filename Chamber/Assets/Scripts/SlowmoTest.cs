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

    private void Start() {
        normalColor = uiElement.color;
        amount = maxDuration;
    }
    void Update()
    {
        if (!GameObject.Find("GameManager").GetComponent<GameManager>().paused) {
            if (Input.GetButton("Fire2") && !cooldown) {
                Time.timeScale = peakSlowmo;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                amount -= Time.unscaledDeltaTime;
                if (amount <= 0) {
                    cooldown = true;
                    amount = 0;
                }
            } else {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
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
}
