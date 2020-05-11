using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
public class OptionsManager : MonoBehaviour {

    [System.Serializable]
    public enum SettingsSubcategory {
        Audio,
        Video,
        Gameplay,
        Control
    };

    #region Templates
    public Transform subcategoryTitleTemplate;
    public Transform sliderOptionTemplate;
    public Transform toggleOptionTemplate;
    public Transform dropdownOptionTemplate;
    #endregion

    #region Viewports
    public GameObject audioViewport;
    public Transform audioContent;
    public GameObject videoViewport;
    public Transform videoContent;
    public GameObject gameplayViewport;
    public Transform gameplayContent;
    public GameObject controlViewport;
    public Transform controlContent;
    #endregion

    public float subcategoryHeight = 64.0f;
    public float fieldHeight = 56.0f;

    #region Private
    private Transform category;
    private float ypos = 0.0f;
    private FullScreenMode fullscreenMode;
    #endregion

    void Awake() {
        AddAudio();
        AddVideo();
        AddGameplay();
        AddControls();
    }

    public void SwapToSubcategory(int subcat) {
        switch(subcat) {
            case 0:
                audioViewport.SetActive(true);
                controlViewport.SetActive(false);
                gameplayViewport.SetActive(false);
                videoViewport.SetActive(false);
            break;
            case 1:
                audioViewport.SetActive(false);
                controlViewport.SetActive(false);
                gameplayViewport.SetActive(false);
                videoViewport.SetActive(true);
            break;
            case 2:
                audioViewport.SetActive(false);
                controlViewport.SetActive(false);
                gameplayViewport.SetActive(true);
                videoViewport.SetActive(false);
            break;
            case 3:
                audioViewport.SetActive(false);
                controlViewport.SetActive(true);
                gameplayViewport.SetActive(false);
                videoViewport.SetActive(false);
            break;
        }
    }
    
    public delegate void IntDelegate(int x);
    public delegate void FloatDelegate(float x);
    public delegate void BoolDelegate(bool x);

    #region Type Creators and Methods
    void CreateSubcategory(string text) {
        RectTransform t = Instantiate(subcategoryTitleTemplate, category) as RectTransform;
        t.GetComponent<Text>().text = text;
        t.anchoredPosition = new Vector2(t.anchoredPosition.x, -ypos);
        ypos += subcategoryHeight;
    }

    void SliderFloatChange(InputField i, Slider s, string optionName, FloatDelegate del) {
        PlayerPrefs.SetFloat(optionName, s.value);
        i.text = s.value.ToString();
        del(s.value);
    }

    void SliderFloatTextChange(InputField i, Slider s, string optionName, FloatDelegate del) {
        float val;
        if(float.TryParse(i.text, out val)) {
            if (val > s.maxValue) {
                val = s.maxValue;
                i.text = val.ToString();
            }
            else if (val < s.minValue) {
                val = s.minValue;
                i.text = val.ToString();
            }
            s.value = val;
            PlayerPrefs.SetFloat(optionName, s.value);
            del(s.value);
        }
    }

    void CreateSlider(string text, string optionName, float min, float max, float defaultValue, FloatDelegate del) {
        RectTransform t = Instantiate(sliderOptionTemplate, category) as RectTransform;
        t.anchoredPosition = new Vector2(t.anchoredPosition.x, -ypos);
        t.GetChild(0).GetComponent<Text>().text = text;
        Slider s = t.GetChild(1).GetComponent<Slider>();
        InputField inputField = t.GetChild(2).GetComponent<InputField>();
        s.minValue = min;
        s.maxValue = max;
        s.value = PlayerPrefs.GetFloat(optionName, defaultValue);
        s.onValueChanged.AddListener(delegate {SliderFloatChange(inputField, s, optionName, del);});
        inputField.text = s.value.ToString();
        inputField.onValueChanged.AddListener(delegate {SliderFloatTextChange(inputField, s, optionName, del);});
        ypos += fieldHeight;
    }

    void SliderIntChange(InputField i, Slider s, string optionName, IntDelegate del) {
        int v =  (int)s.value;
        PlayerPrefs.SetInt(optionName, v);
        i.text = v.ToString();
        del(v);
    }

    void SliderIntTextChange(InputField i, Slider s, string optionName, IntDelegate del) {
        int val;
        if(int.TryParse(i.text, out val)) {
            if (val > s.maxValue) {
                val = (int)s.maxValue;
            }
            else if (val < s.minValue) {
                val = (int)s.minValue;
            }
            
            i.text = val.ToString();
            s.value = val;
            PlayerPrefs.SetInt(optionName, val);
            del(val);
        }
    }

    void CreateSliderInt(string text, string optionName, int min, int max, int defaultValue, IntDelegate del) {
        RectTransform t = Instantiate(sliderOptionTemplate, category) as RectTransform;
        t.anchoredPosition = new Vector2(t.anchoredPosition.x, -ypos);
        t.GetChild(0).GetComponent<Text>().text = text;
        Slider s = t.GetChild(1).GetComponent<Slider>();
        InputField inputField = t.GetChild(2).GetComponent<InputField>();
        s.wholeNumbers = true;
        s.minValue = min;
        s.maxValue = max;
        s.value = PlayerPrefs.GetInt(optionName, defaultValue);
        s.onValueChanged.AddListener(delegate {SliderIntChange(inputField, s, optionName, del);});
        inputField.contentType = InputField.ContentType.IntegerNumber;
        inputField.text = s.value.ToString();
        inputField.onValueChanged.AddListener(delegate {SliderIntTextChange(inputField, s, optionName, del);});
        ypos += fieldHeight;
    }

    void ToggleChange(Toggle t, string optionName, BoolDelegate del) {
        PlayerPrefs.SetInt(optionName, t.isOn ? 1 : 0);
        del(t.isOn);
    }

    void CreateToggle(string text, string optionName, bool defaultValue, BoolDelegate del) {
        RectTransform t = Instantiate(toggleOptionTemplate, category) as RectTransform;
        t.anchoredPosition = new Vector2(t.anchoredPosition.x, -ypos);
        t.GetChild(0).GetComponent<Text>().text = text;
        Toggle toggle = t.GetChild(1).GetComponent<Toggle>();
        toggle.isOn = (PlayerPrefs.GetInt(optionName, defaultValue ? 1 : 0) != 0) ? true : false;
        toggle.onValueChanged.AddListener(delegate {ToggleChange(toggle, optionName, del);});
        ypos += fieldHeight;
    }

    void FinishCategory() {
        RectTransform rt = (RectTransform)category;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, ypos);
        ypos = 0;
    }

    void DropdownChange(Dropdown dd, string optionName, IntDelegate callback) {
        PlayerPrefs.SetInt(optionName, dd.value);
        callback(dd.value);
    }

    void CreateDropDownList(string text, string optionName, List<string> optionList, int defaultValue, IntDelegate callback) {
        RectTransform t = Instantiate(dropdownOptionTemplate, category) as RectTransform;
        t.anchoredPosition = new Vector2(t.anchoredPosition.x, -ypos);
        t.GetChild(0).GetComponent<Text>().text = text;
        Dropdown dd = t.GetChild(1).GetComponent<Dropdown>();
        dd.AddOptions(optionList);
        dd.value = PlayerPrefs.GetInt(optionName, defaultValue);
        dd.RefreshShownValue();
        dd.onValueChanged.AddListener(delegate {DropdownChange(dd, optionName, callback);});

        ypos += fieldHeight;
    }
    #endregion

    #region Categories
    void AddAudio() {
        category = audioContent;
        CreateSubcategory("Volume");
        CreateSliderInt("Master Volume", "volume-master"        , 0, 100, 100, masterVolumeCallback);
        CreateSliderInt("Music Volume", "volume-music"          , 0, 100, 100, musicVolumeCallback);
        CreateSliderInt("Narration Volume", "volume-narration"  , 0, 100, 100, narrationVolumeCallback);
        CreateSliderInt("SFX Volume", "volume-sfx"              , 0, 100, 100, sfxVolumeCallback);
        CreateSubcategory("Subtitles");
        CreateToggle("Subtitles Enabled", "subtitle-enable",    true, subtitlesEnableCallback);
        FinishCategory();
    }

    void AddVideo() {
        category = videoContent;
        CreateSubcategory("Display");
        Resolution curres = Screen.currentResolution;
        int curres_index = 0;
        List<string> reslist = new List<string>();
        for(int i = 0; i < Screen.resolutions.Length; ++i) {
            Resolution res = Screen.resolutions[i];
            reslist.Add(res.width + " x " + res.height);
            if (res.width == curres.width && res.height == curres.height) {
                curres_index = i;
            }
        }
        curres_index = Screen.resolutions.Length - 1 - curres_index;
        reslist.Reverse();
        CreateDropDownList("Resolution", "resolution", reslist, curres_index, resolutionCallback);
        List<string> fullScreenModes = new List<string>();
        fullScreenModes.Add("Fullscreen");
        fullScreenModes.Add("Borderless Window");
        fullScreenModes.Add("Windowed");
        int fsmode = 0;
        switch(Screen.fullScreenMode) {
            case FullScreenMode.ExclusiveFullScreen:
                fsmode = 0;
                break;
            case FullScreenMode.FullScreenWindow:
                fsmode = 1;
                break;
            case FullScreenMode.Windowed:
                fsmode = 2;
                break;
        }
        CreateDropDownList("Full Screen", "fullscreen", fullScreenModes, fsmode, fullscreenCallback);
        CreateSubcategory("Graphics");
        List<string> qualities = new List<string>();
        qualities.Add("Very Low");
        qualities.Add("Low");
        qualities.Add("Medium");
        qualities.Add("High");
        qualities.Add("Very High");
        qualities.Add("Ultra");
        CreateDropDownList("Graphics Quality", "quality", qualities, QualitySettings.GetQualityLevel(), graphicsQualityCallback);
        CreateSliderInt("Number of Decals", "decal-num"        , 0, 1000, 100, decalCallback);
        CreateSliderInt("Field of View", "fov"                 , 45, 120, 60, fovCallback);
        FinishCategory();
    }

    void AddGameplay() {
        category = gameplayContent;
        FinishCategory();
    }

    void AddControls() {
        category = controlContent;
        FinishCategory();
    }
    #endregion

    #region Setting Callbacks
    void masterVolumeCallback(int v) {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        bus.setVolume(v / 100.0f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitGeneric");
    }
    void musicVolumeCallback(int v) {

    }
    void narrationVolumeCallback(int v) {

    }
    void sfxVolumeCallback(int v) {

    }
    
    void subtitlesEnableCallback(bool e) {

    }

    void decalCallback(int d) {
        DecalManager dm = FindObjectOfType<DecalManager>();
        if (dm) {
            dm.Resize(d);
        }
    }

    void fovCallback(int fov) {
    }

    void resolutionCallback(int r) {
        int i = Screen.resolutions.Length - 1 - r;
        Resolution res = Screen.resolutions[i];
        Screen.SetResolution(res.width, res.height, fullscreenMode);
    }

    void fullscreenCallback(int f) {
        FullScreenMode fsm = FullScreenMode.Windowed;
        switch(f) {
            case 0:
                fsm = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                fsm = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                fsm = FullScreenMode.Windowed;
                break;
        }
        fullscreenMode = fsm;
        Screen.fullScreenMode = fsm;
    }

    void graphicsQualityCallback(int quality) {
        QualitySettings.SetQualityLevel(quality);
    }
    #endregion
}
