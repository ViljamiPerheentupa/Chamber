using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float mouseX;
    float mouseY;
    float xRotation = 0f;
    public bool inverted = false;
    public Vector3 mouseKick;
    public Vector3 screenShake;
    private float nextScreenShake;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked; //Lock the cursor (Makes it invisible, and so it can't escape the game window)
    }

    public void StartReset(float xangle) {
        mouseX = xangle;
        mouseY = 0.0f;
        xRotation = 0.0f;
    }

    void Update() {
        mouseKick *= (1 - Time.deltaTime);
        if (Time.time > nextScreenShake) {
            mouseKick.x += Random.Range(-screenShake.x, screenShake.x);
            mouseKick.y += Random.Range(-screenShake.y, screenShake.y);
            mouseKick.z += Random.Range(-screenShake.z, screenShake.z);
            nextScreenShake = Time.time + Random.Range(0.01f, 0.07f);
        }

        if (!GameObject.Find("GameManager").GetComponent<GameManager>().paused) {
            mouseX += Input.GetAxisRaw("Mouse X") * mouseSensitivity; //Get the mouse X and Y axis'
            if (inverted) {
                mouseY = -Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            } else mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90); //Don't let the player do a 360 in a Y axis, so they can't look behind them

            transform.localRotation = Quaternion.Euler(mouseKick.y + xRotation, 0, mouseKick.z);
            transform.parent.rotation = Quaternion.Euler(0, mouseKick.x + mouseX, 0);
            //transform.rotation = Quaternion.Euler(xRotation, mouseX, 0); //Rotate the player based on input
        }
    }
}
