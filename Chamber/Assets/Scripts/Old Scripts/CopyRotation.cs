using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform target;
    public Rigidbody rig;

    void FixedUpdate()
    {
        if (!GetComponent<PlayerMover>().sliding) {
            rig.MoveRotation(Quaternion.Euler(0, target.transform.rotation.eulerAngles.y, 0));
        }
    }
}
