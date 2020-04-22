using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject inGameUI;
    public bool paused;
    FMOD.Studio.EventInstance pausefilter;
    private void Update() {
        if(Input.GetButtonDown("Cancel")) {
            Pause();
        }
    }
    private void Start() {
        pauseMenu = GameObject.Find("PauseMenu");
        inGameUI = GameObject.Find("InGameUI");
        pauseMenu.gameObject.SetActive(false);
    }
    public void Pause() {
        if (!paused) {
            pausefilter = FMODUnity.RuntimeManager.CreateInstance("snapshot:/Pause");
            pausefilter.start();
            pausefilter.release();
        } else pausefilter.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        Time.timeScale = paused ? 1 : 0;
        Cursor.lockState = paused ? CursorLockMode.Locked : CursorLockMode.Confined;
        Cursor.visible = paused ? false : true;
        pauseMenu.SetActive(!paused);
        inGameUI.SetActive(paused);
        paused = !paused;
    }
    public void LoadScene(int scene) {
        GameObject.Find("MusicManager").GetComponent<MusicManager>().StopMusic();
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }
}
