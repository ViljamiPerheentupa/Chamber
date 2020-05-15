using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    float mouseX;
    float mouseY;
    float xRotation = 0f;
    float yRotation = 0f;
    public Vector2 lookAxis;
    public bool inverted = false;
    public Vector3 mouseKick;
    public Vector3 screenShake;
    public bool limitX;
    public float yLiminMin = -90f;
    public float yLiminMax = 90f;
    public float xLiminMin = -60f;
    public float xLiminMax = 60f;
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
            mouseX = lookAxis.x * mouseSensitivity * Time.deltaTime; //Get the mouse X and Y axis'
            mouseY = lookAxis.y * mouseSensitivity * Time.deltaTime;
            if (inverted) mouseY *= 1;

            xRotation += mouseX;
            if (limitX) {
                xRotation = Mathf.Clamp(xRotation, xLiminMin, xLiminMax);
            }
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, yLiminMin, yLiminMax); //Don't let the player do a 360 in a Y axis, so they can't look behind them

            transform.localRotation = Quaternion.Euler(mouseKick.y + yRotation, 0, mouseKick.z);
            transform.parent.rotation = Quaternion.Euler(0, mouseKick.x + xRotation, 0);
            //transform.rotation = Quaternion.Euler(xRotation, mouseX, 0); //Rotate the player based on input
        }
    }
}
