using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform target;
    public Rigidbody rig;

    void FixedUpdate()
    {
        rig.MoveRotation(Quaternion.Euler(0, target.transform.rotation.eulerAngles.y, 0));
    }
}
