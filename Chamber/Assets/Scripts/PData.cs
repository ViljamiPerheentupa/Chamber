using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PData : MonoBehaviour
{
    float mouseSensitivity;
    private void Awake() {
        mouseSensitivity = 1;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("PData");
        if (objs.Length > 1) {
            for (int i = 0; i < objs.Length - 1; i++) {
                Destroy(objs[i + 1]);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        mouseSensitivity = GameObject.Find("MouseSensitivity").GetComponent<Slider>().value;
        if (Camera.main != null) {
            if (Camera.main.GetComponent<MouseLook>() != null) {
                if (Camera.main.GetComponent<MouseLook>().mouseSensitivity != mouseSensitivity) Camera.main.GetComponent<MouseLook>().mouseSensitivity = mouseSensitivity;
            }
        }
    }
}
