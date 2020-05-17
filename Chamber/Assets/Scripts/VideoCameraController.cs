using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCameraController : MonoBehaviour {
    public float rotSpeed = 1f;

    void Update() {
        Transform tr = Camera.main.transform;
        Vector3 diff = tr.position - transform.position;
        Quaternion targetQuat = Quaternion.LookRotation(Vector3.Normalize(diff));;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuat, rotSpeed * Time.deltaTime);
    }
}
