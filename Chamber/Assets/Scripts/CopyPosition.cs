using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform target;
    public Rigidbody rig;
    void FixedUpdate()
    {
        rig.MovePosition(target.GetComponent<Rigidbody>().position);
    }
}
