using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject inGameUI;
    bool paused;
    private void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            Pause();
        }
    }
    private void Start() {
        pauseMenu = GameObject.Find("PauseMenu");
        inGameUI = GameObject.Find("InGameUI");
        pauseMenu.gameObject.SetActive(false);
    }
    public void Pause() {
        Time.timeScale = paused ? 1 : 0;
        pauseMenu.SetActive(!paused);
        inGameUI.SetActive(paused);
        paused = !paused;
    }
    public void LoadScene(int scene) {
        SceneManager.LoadScene(scene);
    }
}
