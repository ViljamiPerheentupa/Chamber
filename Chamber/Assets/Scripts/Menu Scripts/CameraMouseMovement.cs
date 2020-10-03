using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseMovement : MonoBehaviour
{
    Vector3 zeroPoint;
    float i;
    float j;
    Vector3 rMousePos;
    Vector3 normalRotation;
    public AnimationCurve curve;

    void Start()
    {
        zeroPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        i = Screen.width / 4;
        j = Screen.height / 4;
        normalRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MouseOffScreen()) {
            rMousePos = Input.mousePosition - zeroPoint;
            var x = rMousePos.x / i;
            var y = rMousePos.y / j;
            var targetRotation = normalRotation + new Vector3(-y * 2, x * 2, 0);
            transform.localRotation = Quaternion.Euler(targetRotation);
        } /*else transform.rotation = Quaternion.Euler(normalRotation);*/
    }


    bool MouseOffScreen() {
        if (Input.mousePosition.y > Screen.height || Input.mousePosition.y < 0) {
            return true;
        } else if (Input.mousePosition.x > Screen.width || Input.mousePosition.x < 0) {
            return true;
        } else return false;
    }
}
