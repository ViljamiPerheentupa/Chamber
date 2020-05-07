using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElevatorLoader : MonoBehaviour {
    public LayerMask playerLayerMask = (1 << 9);
    public string sceneNameA;
    public string sceneNameB;
    public DoorSlider doorA;
    public DoorSlider doorB;
    public bool elevatorCleared = true;

    public void LeftElevator() {
        elevatorCleared = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (elevatorCleared) {
            elevatorCleared = false;

            GameObject sceneManagerObj = GameObject.Find("SceneManager");
            if (sceneManagerObj) {
                SceneManagerUtil sceneManager = sceneManagerObj.GetComponent<SceneManagerUtil>();
                if (sceneManager) {
                    if (SceneManager.GetSceneByName(sceneNameB).isLoaded) {
                        sceneManager.LoadScene(sceneNameA);
                        doorB.Close();

                        // Do this on loadscene complete
                        doorA.Open();

                        // Unload scene after door close
                        sceneManager.UnloadScene(sceneNameB);
                    }
                    else {
                        sceneManager.LoadScene(sceneNameB);
                        doorA.Close();
                        
                        // Do this on loadscene complete
                        doorB.Open();
                        
                        // Unload scene after door close
                        sceneManager.UnloadScene(sceneNameA);
                    }
                }
            }
        }
    }
}
