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
    [Tooltip("The minimum time to wait before opening the door (in case the level loads too fast).")]
    public float minimumWaitTime = 1.0f;
    [Tooltip("If checked, you can only go from A to B")]
    public bool oneWay;
    public float shakeAmt = 0.3f;

    private SceneManagerUtil sceneManager;
    private float openDoorTime;

    void Start() {
        GameObject sceneManagerObj = GameObject.Find("SceneManager");
        if (sceneManagerObj) {
            sceneManager = sceneManagerObj.GetComponent<SceneManagerUtil>();
        }
    }

    public void LeftElevator() {
        elevatorCleared = true;
    }

    void UnloadSceneA() {
        MouseLook ml = GameObject.FindObjectOfType<MouseLook>();
        if (ml) {
            ml.screenShake = new Vector3(shakeAmt, shakeAmt, shakeAmt);
        }
        
        sceneManager.UnloadScene(sceneNameA);
    }

    void UnloadSceneB() {
        MouseLook ml = GameObject.FindObjectOfType<MouseLook>();
        if (ml) {
            ml.screenShake = new Vector3(shakeAmt, shakeAmt, shakeAmt);
        }

        sceneManager.UnloadScene(sceneNameB);
    }

    IEnumerator WaitToOpen(AsyncOperation asyncOperation, DoorSlider slider) {
        while (!asyncOperation.isDone || Time.time < openDoorTime) {
            yield return null;
        }

        MouseLook ml = GameObject.FindObjectOfType<MouseLook>();
        if (ml) {
            ml.screenShake = new Vector3(0, 0, 0);
        }

        slider.Open();
    }

    private void OnTriggerEnter(Collider other) {
        if (elevatorCleared) {
            if (sceneManager) {
                elevatorCleared = false;
                if (SceneManager.GetSceneByName(sceneNameA).isLoaded) {
                    AsyncOperation asyncOperation = sceneManager.LoadScene(sceneNameB);
                    if (asyncOperation != null) {
                        openDoorTime = Time.time + minimumWaitTime;
                        doorA.Close();

                        // Unload scene after door close
                        Invoke("UnloadSceneA", doorA.timeToMove);

                        // Do this on loadscene complete
                        StartCoroutine(WaitToOpen(asyncOperation, doorB));
                    }
                    else {
                        Debug.LogError("Unable to load scene: " + sceneNameB);
                    }
                }
                else if (!oneWay) {
                    openDoorTime = Time.time + minimumWaitTime;
                    AsyncOperation asyncOperation = sceneManager.LoadScene(sceneNameA);
                    if (asyncOperation != null) {
                        doorB.Close();
                        
                        // Unload scene after door close
                        Invoke("UnloadSceneB", doorB.timeToMove);
                        
                        // Do this on loadscene complete
                        StartCoroutine(WaitToOpen(asyncOperation, doorA));
                    }
                    else {
                        Debug.LogError("Unable to load scene: " + sceneNameA);
                    }
                }
            }
        
        }
    }
}
