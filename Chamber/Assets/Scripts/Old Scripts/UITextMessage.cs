using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextMessage : MonoBehaviour, IUIMessage
{
    TextMeshProUGUI uiText;
    string targetText;
    float targetAlpha;
    public float messageDurationDefault = 10f;
    public AnimationCurve fadeIn;
    float fadeInTimer = 0;
    public AnimationCurve fadeOut;
    float fadeOutTimer = 0;
    float timer = 0;
    float messageDuration;
    bool message = false;
    bool fadedIn = false;
    bool fadedOut = true;
    void Start()
    {
        uiText = GameObject.Find("UITextMessage").GetComponent<TextMeshProUGUI>();
        targetText = "";
        targetAlpha = uiText.color.a;
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, 0);
    }

    void Update()
    {
        if (targetText != "" && !message) {
            WriteMessage();
        }
        if (message) {
            if (targetText != "") {
                WriteMessage();
            }
            if (!fadedIn) {
                FadeIn();
            } else {
                timer += Time.deltaTime;
                if (timer >= messageDuration && !fadedOut) {
                    FadeOut();
                }
            }
        }
    }

    void FadeIn() {
        fadeInTimer += Time.deltaTime;
        float t = fadeIn.Evaluate(fadeInTimer);
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, t);
        if (fadeInTimer >= 1f) {
            uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, targetAlpha);
            fadedIn = true;
            fadedOut = false;
        }
    }

    void FadeOut() {
        fadeOutTimer += Time.deltaTime;
        float t = fadeOut.Evaluate(fadeOutTimer);
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, t);
        if (fadeOutTimer >= 1f) {
            ResetText();
        }
    }

    void ResetText() {
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, 0);
        fadedIn = false;
        fadedOut = true;
        message = false;
        fadeOutTimer = 0;
        fadeInTimer = 0;
        timer = 0;
        uiText.text = "";
    }

    void WriteMessage() {
        ResetText();
        uiText.text = targetText;
        targetText = "";
        message = true;
    }

    public void UIMessage(string text, float duration) {
        if (duration != 0) {
            messageDuration = duration;
        } else messageDuration = messageDurationDefault;
        targetText = text;
    }
}
