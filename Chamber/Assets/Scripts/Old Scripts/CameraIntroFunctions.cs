using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntroFunctions : MonoBehaviour
{
    public Animator gun;
    public GameObject gunCamera;
    public void Ready() {
        gunCamera.SetActive(true);
        gun.Play("gun_ready");
    }

    public void DisableAnimator() {
        GetComponent<Animator>().enabled = false;
        GetComponent<MouseLook>().enabled = true;
        GetComponent<MouseLook>().limitX = false;
    }
}
