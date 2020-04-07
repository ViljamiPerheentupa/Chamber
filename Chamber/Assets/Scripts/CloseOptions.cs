using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOptions : MonoBehaviour
{
    bool options = false;
    private void Update() {
        if (Input.GetButtonDown("Cancel") && options) {
            GameObject.Find("MenuManager").GetComponent<MenuManager>().OptionsMenuClose();
        }
    }

    public void OptionsOn() {
        options = true;
    }

    public void OptionsOff() {
        options = false;
    }
}
