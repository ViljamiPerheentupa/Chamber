using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeSceneTrigger : MonoBehaviour {
    public LayerMask contactLayerMask = (1 << 9);
    public List<string> loadScenes = new List<string>();
    public List<string> unloadScenes = new List<string>();

    private void OnTriggerEnter(Collider other) {
        GameObject sceneManagerObj = GameObject.Find("SceneManager");
        if (sceneManagerObj) {
            SceneManagerUtil sceneManager = sceneManagerObj.GetComponent<SceneManagerUtil>();
            if (sceneManager) {
                foreach(string st in loadScenes) {
                    sceneManager.LoadScene(st);
                }

                foreach(string st in unloadScenes) {
                    sceneManager.UnloadScene(st);
                }
            }
        }
    }
}
