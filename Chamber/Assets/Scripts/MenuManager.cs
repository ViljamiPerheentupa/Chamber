using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public Animator bgAnim;
    public Animator hdAnim;
    public MenuButtonEnabler[] buttons;
    public AudioMixerSnapshot outro;
    public void StartGame() {
        bgAnim.Play("MenuOutro");
        hdAnim.Play("HeaderOutro");
        outro.TransitionTo(2f);
        foreach (MenuButtonEnabler button in buttons) {
            button.FadeOut();
        }
    }


    public void QuitGame() {
        Application.Quit();
    }
}
