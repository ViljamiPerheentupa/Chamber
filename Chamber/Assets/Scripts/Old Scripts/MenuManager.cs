
#pragma warning disable CS0414

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Animator bgAnim;
    public Animator hdAnim;
    public MenuButtonEnabler[] buttons;
    public AudioMixerSnapshot outro;
    public Animator opAnim;
    public Button opMenuCloser;
    bool outroGoing = false;

    public float mouseSensitivity;
    public float volumeMaster;
    public float volumeMusic;
    public float volumeSFX;
    FMOD.Studio.VCA masterVolume;
    FMOD.Studio.VCA musicVolume;
    FMOD.Studio.VCA sfxVolume;

    public float decalLimit;
    public bool mInverse = false;

    FMOD.Studio.EventInstance Music;

    public bool menu = true;

    public GameObject devTL;
    public GameObject devGG;

    private void Awake() {
        
        /*if (PlayerPrefs.GetInt("HasSettings") != 1) {
            print("Creating settings");
            volumeMaster = GameObject.Find("MasterVolume").GetComponent<Slider>().value;
            volumeMusic = GameObject.Find("MusicVolume").GetComponent<Slider>().value;
            volumeSFX = GameObject.Find("SFXVolume").GetComponent<Slider>().value;
            decalLimit = GameObject.Find("DecalLimit").GetComponent<Slider>().value;
            mouseSensitivity = GameObject.Find("MouseSensitivity").GetComponent<Slider>().value;
            PlayerPrefs.SetFloat("vMaster", volumeMaster);
            PlayerPrefs.SetFloat("vMusic", volumeMusic);
            PlayerPrefs.SetFloat("vSFX", volumeSFX);
            PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
            PlayerPrefs.SetFloat("dLimit", decalLimit);
            PlayerPrefs.SetInt("mInverse", 0);
            PlayerPrefs.SetInt("HasSettings", 1);
            PlayerPrefs.Save();
        } else {
            print("Fetched settings " + PlayerPrefs.GetInt("mInverse"));
            volumeMaster = PlayerPrefs.GetFloat("vMaster");
            volumeMusic = PlayerPrefs.GetFloat("vMusic");
            volumeSFX = PlayerPrefs.GetFloat("vSFX");
            decalLimit = PlayerPrefs.GetFloat("dLimit");
            mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
            if (PlayerPrefs.GetInt("mInverse") != 0) {
                GameObject.Find("InvertMouse").GetComponent<Toggle>().isOn = true;
                mInverse = true;
            } else {
                GameObject.Find("InvertMouse").GetComponent<Toggle>().isOn = false;
                mInverse = false;
            }
        }*/
    }
    private void Start() {
        if (menu) {
            Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Main Menu");
            Music.start();
        }
        //opMenuCloser = GameObject.FindGameObjectWithTag("OptionsCloser").GetComponent<Button>();
        //opAnim = GameObject.FindGameObjectWithTag("Options").GetComponent<Animator>();
        //pdata = GameObject.FindGameObjectWithTag("PData").GetComponent<PData>();
    }

    private void Update() {
        /*if (GameObject.Find("MouseSensitivity").GetComponent<Slider>().value != mouseSensitivity) GameObject.Find("MouseSensitivity").GetComponent<Slider>().value = mouseSensitivity;
        if (GameObject.Find("DecalLimit").GetComponent<Slider>().value != decalLimit) GameObject.Find("DecalLimit").GetComponent<Slider>().value = decalLimit;
        if (GameObject.Find("MasterVolume").GetComponent<Slider>().value != volumeMaster) GameObject.Find("MasterVolume").GetComponent<Slider>().value = volumeMaster;
        if (GameObject.Find("MusicVolume").GetComponent<Slider>().value != volumeMusic) GameObject.Find("MusicVolume").GetComponent<Slider>().value = volumeMusic;
        if (GameObject.Find("SFXVolume").GetComponent<Slider>().value != volumeSFX) GameObject.Find("SFXVolume").GetComponent<Slider>().value = volumeSFX;
        masterVolume.setVolume(volumeMaster);
        musicVolume.setVolume(volumeMusic);
        sfxVolume.setVolume(volumeSFX);
        if (Camera.main != null) {
            if (Camera.main.GetComponent<MouseLook>() != null) {
                if (Camera.main.GetComponent<MouseLook>().mouseSensitivity != mouseSensitivity) Camera.main.GetComponent<MouseLook>().mouseSensitivity = mouseSensitivity;
                if (mInverse) {
                    Camera.main.GetComponent<MouseLook>().inverted = true;
                } else Camera.main.GetComponent<MouseLook>().inverted = false;
            }
        }
        if (GameObject.Find("Decals") != null) {
            GameObject.Find("Decals").GetComponent<DecalManager>().decalLimit = Mathf.FloorToInt(decalLimit);
        }
        if (!GameObject.Find("MenuManager").GetComponent<MenuManager>().menu) {
            GameObject.Find("DecalLimit").GetComponent<Slider>().interactable = false;
        } else GameObject.Find("DecalLimit").GetComponent<Slider>().interactable = true;*/
    }

    public void StartGame() {
        outroGoing = true;
        bgAnim.Play("MenuOutro");
        hdAnim.Play("HeaderOutro");
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Music.release();
        foreach (MenuButtonEnabler button in buttons) {
            button.FadeOut();
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void UISelectSound() {
        FMOD.Studio.EventInstance selectSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/UISelect");
        selectSound.start();
        selectSound.release();
    }

    public void UIClickSound() {
        FMOD.Studio.EventInstance clickSound = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/UIClick");
        clickSound.start();
        clickSound.release();
    }
}
