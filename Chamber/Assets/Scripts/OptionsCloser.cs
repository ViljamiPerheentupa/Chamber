using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsCloser : MonoBehaviour
{
    public void CloseOptions() {
        GameObject.Find("MenuManager").GetComponent<MenuManager>().OptionsMenuClose();
    }
}
