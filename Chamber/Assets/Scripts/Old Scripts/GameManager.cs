using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject pauseMenu;
    public GameObject inGameUI;
    public bool paused;
    FMOD.Studio.EventInstance pausefilter;
    private void Update() {
        if(Input.GetButtonDown("Cancel")) {
            Pause();
        }
    }
    private void Start() {
        if (optionsMenu) {
            OptionsManager om = optionsMenu.GetComponent<OptionsManager>();
            if (om) {
                om.LoadPrefs();
            }
        }
        
        pauseMenu.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        Cursor.visible = false;
    }

    public void Pause() {
        if (!paused) {
            if (FindObjectOfType<PlayerHealth>().isDead) {
                return;
            }

            pausefilter = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Pause");
            pausefilter.start();
            pausefilter.release();
        } else pausefilter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Time.timeScale = paused ? 1 : 0;
        Cursor.lockState = paused ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = paused ? false : true;
        optionsMenu.SetActive(false);
        pauseMenu.GetComponent<PauseMenuAnimHandler>().StartFade(!paused);
        inGameUI.SetActive(paused);
        paused = !paused;
    }
    
    public void LoadScene(int scene) {
        GameObject.Find("MusicManager").GetComponent<MusicManager>().StopMusic();
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }
}
