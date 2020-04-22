using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PData : MonoBehaviour
{
    public float mouseSensitivity;
    public float volumeMaster;
    public float volumeMusic;
    public float volumeSFX;
    FMOD.Studio.VCA masterVolume;
    FMOD.Studio.VCA musicVolume;
    FMOD.Studio.VCA sfxVolume;

    public float decalLimit;
    private void Awake() {
        masterVolume = FMODUnity.RuntimeManager.GetVCA("VCA:/Master");
        musicVolume = FMODUnity.RuntimeManager.GetVCA("VCA:/Music");
        sfxVolume = FMODUnity.RuntimeManager.GetVCA("VCA:/SFX");
        if (PlayerPrefs.GetInt("HasSettings") != 1) {
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
            PlayerPrefs.SetInt("HasSettings", 1);
            PlayerPrefs.Save();
        } else {
            volumeMaster = PlayerPrefs.GetFloat("vMaster");
            volumeMusic = PlayerPrefs.GetFloat("vMusic");
            volumeSFX = PlayerPrefs.GetFloat("vSFX");
            decalLimit = PlayerPrefs.GetFloat("dLimit");
            mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
        }
    }

    void Update()
    {
        if (GameObject.Find("MouseSensitivity").GetComponent<Slider>().value != mouseSensitivity) GameObject.Find("MouseSensitivity").GetComponent<Slider>().value = mouseSensitivity;
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
            }
        }
        if (GameObject.Find("Decals") != null) {
            GameObject.Find("Decals").GetComponent<DecalManager>().decalLimit = Mathf.FloorToInt(decalLimit);
        }
        if (!GameObject.Find("MenuManager").GetComponent<MenuManager>().menu) {
            GameObject.Find("DecalLimit").GetComponent<Slider>().interactable = false;
        } else GameObject.Find("DecalLimit").GetComponent<Slider>().interactable = true;
    }

    public void UpdateMSensitivity() {
        mouseSensitivity = GameObject.Find("MouseSensitivity").GetComponent<Slider>().value;
    }

    public void UpdateVolumeMaster() {
        volumeMaster = GameObject.Find("MasterVolume").GetComponent<Slider>().value;
    }

    public void UpdateVolumeMusic() {
        volumeMusic = GameObject.Find("MusicVolume").GetComponent<Slider>().value;
    }

    public void UpdateVolumeSFX() {
        volumeSFX = GameObject.Find("SFXVolume").GetComponent<Slider>().value;
    }

    public void UpdateDecalLimit() {
        decalLimit = GameObject.Find("DecalLimit").GetComponent<Slider>().value;
    }
}
