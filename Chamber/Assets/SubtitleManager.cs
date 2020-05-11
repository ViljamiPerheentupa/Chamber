using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour {
    public Color color;
    public float fadeDuration = 1.0f;
    Text textComponent;
    float textAppearTime = -10.0f;
    float textDuration;
    public bool showSubtitles;

    private void Start() {
        textComponent = transform.GetComponent<Text>();
        textComponent.enabled = false;
    }
    
    public void AddSubtitle(string text) {
        float duration = 4.0f;

        if (showSubtitles) {
            textComponent.enabled = true;
            textComponent.text = text;
            textDuration = duration;
            textAppearTime = Time.time;
            textComponent.color = color;
        }
    }

    // Update is called once per frame
    void Update() {
        if (Time.time > textAppearTime + textDuration) {
            if (Time.time < textAppearTime + textDuration + fadeDuration) {
                float fade = (Time.time - (textAppearTime + textDuration)) / fadeDuration;
                fade = Mathf.Clamp(1.0f - fade, 0.0f, 1.0f);
                textComponent.color = new Color(color.r, color.g, color.b, color.a * fade);
            }
            else {
                textComponent.enabled = false;
            }
        }
    }
}
