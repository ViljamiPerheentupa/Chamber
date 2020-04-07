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

    public Slider mVolume;
    public Slider musicVolume;
    public AudioMixer master;

    FMOD.Studio.EventInstance Music;

    public bool menu = true;

    private void Start() {
        if (menu) {
            Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Main Menu");
            Music.start();
        }
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

    public void OptionsMenu() {
        opAnim.Play("OptionsPopIn");
        if (menu) {
            foreach (MenuButtonEnabler button in buttons) {
                button.GetComponent<Button>().interactable = false;
            }
        }
        opMenuCloser.gameObject.SetActive(true);
    }

    public void OptionsMenuClose() {
        opAnim.Play("OptionsPopOut");
        if (menu) {
            foreach (MenuButtonEnabler button in buttons) {
                button.GetComponent<Button>().interactable = true;
            }
        }
        opMenuCloser.gameObject.SetActive(false);
    }


    public void QuitGame() {
        Application.Quit();
    }

    private void Update() {
        if (!outroGoing) {
            master.SetFloat("Volume", mVolume.value);
            master.SetFloat("musicVol", musicVolume.value);
        }
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
