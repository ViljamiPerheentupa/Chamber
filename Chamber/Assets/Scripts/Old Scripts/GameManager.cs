using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Canvas menuCanvas;
    public Canvas guiCanvas;
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
        guiCanvas.gameObject.SetActive(true);
        menuCanvas.gameObject.SetActive(true);

        if (optionsMenu) {
            OptionsManager om = optionsMenu.GetComponent<OptionsManager>();
            if (om) {
                om.LoadPrefs();
            }

            optionsMenu.SetActive(false);
        }
        
        pauseMenu.gameObject.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        Cursor.visible = false;
    }

    public void Pause() {
        if (!paused) {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph.isDead) {
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
