using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Func_Rotate : MonoBehaviour {
    public Vector3 rotateAxis;
    public float rotateSpeed;

    void SetRotationSpeed(float speed) {
        rotateSpeed = speed;
    }

    void Update() {
        transform.rotation *= Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, rotateAxis);
    }
}
