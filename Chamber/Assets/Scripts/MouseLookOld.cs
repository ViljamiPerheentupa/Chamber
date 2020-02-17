using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookOld : MonoBehaviour
{
    public float mouseSensitivity;
    public Transform body;
    float mouseX;
    float mouseY;
    float xRotation = 0;
    Rigidbody rig;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        body.Rotate(Vector3.up * mouseX);
    }
}
