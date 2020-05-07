using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerUtil : MonoBehaviour {
    public List<string> initialScenes = new List<string>();

    private void Start() {
        foreach(string st in initialScenes) {
            LoadScene(st);
        }
    }

    public void LoadScene(string levelName) {
        if (!SceneManager.GetSceneByName(levelName).isLoaded) {
            Debug.Log("Load: " + levelName);
            SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
        }
    }

    public void UnloadScene(string levelName) {
        if (SceneManager.GetSceneByName(levelName).isLoaded) {
            Debug.Log("Unload: " + levelName);
            SceneManager.UnloadSceneAsync(levelName);
        }
    }
}
