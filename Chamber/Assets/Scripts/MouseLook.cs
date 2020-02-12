using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private float X;
    private float Y;

    public float Sensitivity;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Vector3 euler = transform.rotation.eulerAngles;
        X = euler.x;
        Y = euler.y;
    }

    void Update() {
        const float MIN_X = 0.0f;
        const float MAX_X = 360.0f;
        const float MIN_Y = -90.0f;
        const float MAX_Y = 90.0f;

        X += Input.GetAxis("Mouse X") * (Sensitivity * Time.deltaTime);
        if (X < MIN_X) X += MAX_X;
        else if (X > MAX_X) X -= MAX_X;
        Y -= Input.GetAxis("Mouse Y") * (Sensitivity * Time.deltaTime);
        if (Y < MIN_Y) Y = MIN_Y;
        else if (Y > MAX_Y) Y = MAX_Y;

        transform.rotation = Quaternion.Euler(Y, X, 0.0f);
    }

    //public float mouseSensitivity = 100f;
    //public Transform playerBody;
    //float mouseX;
    //float mouseY;
    //float xRotation = 0f;

    //void Start() {
    //    /*Cursor.lockState = CursorLockMode.Locked;*/ //Lock the cursor (Makes it invisible, and so it can't escape the game window)
    //}
    //void Update() {
    //    mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; //Get the mouse X and Y axis'
    //    mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
    //    print("X: " + Input.GetAxis("Mouse X") + " Y: " + Input.GetAxis("Mouse Y"));

    //    xRotation -= mouseY;
    //    xRotation = Mathf.Clamp(xRotation, -90, 90); //Don't let the player do a 360 in a Y axis, so they can't look behind them

    //    transform.rotation = Quaternion.Euler(xRotation, 0, 0); //Rotate the player based on input
    //    playerBody.Rotate(Vector3.up * mouseX);
    //}
}
