using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Localization : MonoBehaviour {
    #region SINGLETON PATTERN
        public static Localization _instance;
        public static Localization Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Localization>();
                    
                    if (_instance == null) {
                        GameObject container = new GameObject("Localization");
                        _instance = container.AddComponent<Localization>();
                    }
                }
            
                return _instance;
            }
        }
    #endregion

    [System.Serializable]
    public enum Locales {
        English,
        Finnish,
        German,
        Count
    };
    public bool showSubtitles;
    private List<string> localizationPaths;
    private Dictionary<string, string> subtitles = new Dictionary<string, string>();

    private void Awake() {
        localizationPaths = new List<string>();
        
        for(int i = 0; i < (int)Localization.Locales.Count; ++i) {
            string path = ((Localization.Locales)i).ToString();
            path = "localization/" + path;
            localizationPaths.Add(path);
        }

        LoadSubtitles(Locales.English);
    }

    public void LoadSubtitles(Locales language) {
        TextAsset jsonFile = (TextAsset)Resources.Load(localizationPaths[(int)language], typeof(TextAsset));

        subtitles.Clear();
        SubtitleEntryHelperArray jsonInfo = JsonUtility.FromJson<SubtitleEntryHelperArray>(jsonFile.text);
        foreach (SubtitleEntryHelper entry in jsonInfo.List) {
            subtitles.Add(entry.CueName, entry.String);
        }
    }
    
    public string GetTranslation(string text) {
        string value;
        if (subtitles.TryGetValue(text, out value)) {
            return value;
        }
        else {
            Debug.LogError("Error: No such localization found: " + text);
            return null;
        }
    }
    
    public string FormatString(string text) {
        string out_str = text;
        int p = out_str.IndexOf('{');
        int p2;

        while (p != -1) {
            p2 = out_str.IndexOf('}');
            if (p2 == -1) {
                return out_str;
            }
            else {
                string a = out_str.Substring(0, p);
                string b = out_str.Substring(p + 1, p2 - p - 1);
                string c = out_str.Substring(p2 + 1);
                
                b = GetTranslation(b);
                
                out_str = a + b + c;

                p = p2;
            }
        }

        return out_str;
    }
}
