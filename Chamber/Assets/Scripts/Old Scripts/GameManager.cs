using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private static GameManager _instance = null;
    public static GameManager Instance { get {
        if (!_instance) {
            _instance = GameObject.FindObjectOfType<GameManager>();
            if (_instance == null) {
                GameObject container = new GameObject("GameManager");
                _instance = container.AddComponent<GameManager>();
            }
        }

        return _instance;
    }}

    public Canvas menuCanvas;
    public Canvas guiCanvas;
    public GameObject optionsMenu;
    public GameObject pauseMenu;
    public GameObject inGameUI;
    public bool isPaused { get; private set; }
    FMOD.Studio.EventInstance pausefilter;
    
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

    public void TogglePause() {
        if (!isPaused) {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph.isDead) {
                return;
            }

            pausefilter = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Pause");
            pausefilter.start();
            pausefilter.release();
        } else pausefilter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Time.timeScale = isPaused ? 1 : 0;
        Cursor.lockState = isPaused ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = isPaused ? false : true;
        optionsMenu.SetActive(false);
        pauseMenu.GetComponent<PauseMenuAnimHandler>().StartFade(!isPaused);
        inGameUI.SetActive(isPaused);
        isPaused = !isPaused;
    }

    private void OnPause() {
        TogglePause();
    }
    
    public void LoadScene(int scene) {
        GameObject.Find("MusicManager").GetComponent<MusicManager>().StopMusic();
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }
}
