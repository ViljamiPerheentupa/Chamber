using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour {
    [System.Serializable]
    public enum SubtitleLanguage {
        English,
        Finnish,
        German,
        Count
    };
    public Color color;
    public float fadeDuration = 1.0f;
    Text textComponent;
    float textAppearTime = -10.0f;
    float textDuration;
    public bool showSubtitles;
    public TextAsset subtitleEnglishJsonFile;
    public TextAsset subtitleFinnishJsonFile;
    public TextAsset subtitleGermanJsonFile;
    private Dictionary<string, string> subtitles = new Dictionary<string, string>();

    private void Start() {
        textComponent = transform.GetComponent<Text>();
        textComponent.enabled = false;

        LoadSubtitles(SubtitleLanguage.English);
    }

    public void LoadSubtitles(SubtitleLanguage language) {
        TextAsset subtitleJsonFile;

        switch (language) {
            default:
            case SubtitleLanguage.English:
                subtitleJsonFile = subtitleEnglishJsonFile;
                break;
            case SubtitleLanguage.German:
                subtitleJsonFile = subtitleGermanJsonFile;
                break;
            case SubtitleLanguage.Finnish:
                subtitleJsonFile = subtitleFinnishJsonFile;
                break;
        }

        subtitles.Clear();
        SubtitleEntryHelperArray jsonInfo = JsonUtility.FromJson<SubtitleEntryHelperArray>(subtitleJsonFile.text);
        foreach (SubtitleEntryHelper entry in jsonInfo.List) {
            subtitles.Add(entry.CueName, entry.String);
        }
    }
    
    public void AddSubtitle(string text) {
        float duration = 4.0f;

        if (showSubtitles) {
            string value;
            if (subtitles.TryGetValue(text, out value)) {
                textComponent.enabled = true;
                textComponent.text = value;
                textDuration = duration;
                textAppearTime = Time.time;
                textComponent.color = color;
            }
            else {
                Debug.LogError("Error: No such subtitle found: " + text);
            }
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
