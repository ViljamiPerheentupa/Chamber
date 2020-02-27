using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivator : MonoBehaviour
{
    public GameObject[] buttons;
    public void ActivateButtons() {
        foreach (GameObject button in buttons) {
            button.SetActive(true);
            button.GetComponent<MenuButtonEnabler>().FadeIn();
        }
    }
}
