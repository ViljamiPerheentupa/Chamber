using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBGManager : MonoBehaviour
{
    public GameObject camRig;
    public GameObject[] triangles;
    public Vector3 triangleOffset;
    public float cameraForwardSpeed = 0.5f;
    public float cameraRotationSpeed = 0.5f;
    public Vector3 cameraMovement = new Vector3(0, 0, -1);
    public Vector3 cameraRotation = new Vector3(0, 0, 1);
    int index;
    Vector3 offset;
    Quaternion lastTriangleRotation;
    public bool randomRotation = false;
    public float staticTriangleRotation = 15;
    public float minTriangleRotation = 10;
    public float maxTriangleRotation = 20;
    

    void Start()
    {
        camRig = Camera.main.transform.parent.gameObject;
        index = 0;
        offset = triangleOffset * triangles.Length;
        lastTriangleRotation = triangles[triangles.Length - 1].transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        RotateCamera();
    }

    void MoveCamera() {
        camRig.transform.position += cameraMovement * cameraForwardSpeed * Time.deltaTime;
    }

    void RotateCamera() {
        camRig.transform.rotation *= Quaternion.AngleAxis(cameraRotationSpeed * Time.deltaTime, cameraRotation);
    }

    public void RepositionTriangle() {
        var newTriangleRotation = Quaternion.Euler(0, 0, 0);
        if (randomRotation) {
            var randomRotation = lastTriangleRotation.eulerAngles.z + Random.Range(minTriangleRotation, maxTriangleRotation);
            newTriangleRotation = Quaternion.Euler(0, 0, randomRotation);
        } else {
            var newRotation = lastTriangleRotation.eulerAngles.z + staticTriangleRotation;
            newTriangleRotation = Quaternion.Euler(0, 0, newRotation);
        }
        lastTriangleRotation = newTriangleRotation;
        triangles[index].transform.localPosition = offset;
        triangles[index].transform.localRotation = newTriangleRotation;
        offset += triangleOffset;
        index++;
        if (index >= triangles.Length) {
            index = 0;
        }
    }
}
