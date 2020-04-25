using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStopBulletTest : MonoBehaviour
{
    Camera cam;
    public LayerMask layerMask;
    RaycastHit hit;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)){
                if (hit.collider.GetComponent<ITimeLock>() != null) {
                    hit.collider.GetComponent<ITimeLock>().TimeLock();
                } else if (hit.collider.GetComponentInParent<ITimeLock>() != null) {
                    hit.collider.GetComponentInParent<ITimeLock>().TimeLock();
                } else print("Hit object cannot be timelocked");
            }
        }
    }
}
