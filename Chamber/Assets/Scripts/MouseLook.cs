using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float mouseX;
    float mouseY;
    float xRotation = 0f;
    Rigidbody rig;

    void Start() {
        rig = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; //Lock the cursor (Makes it invisible, and so it can't escape the game window)
    }
    void Update() {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity; //Get the mouse X and Y axis'
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90); //Don't let the player do a 360 in a Y axis, so they can't look behind them

        rig.MoveRotation(Quaternion.Euler(xRotation, mouseX, 0));
        //transform.rotation = Quaternion.Euler(xRotation, mouseX, 0); //Rotate the player based on input
    }
}
