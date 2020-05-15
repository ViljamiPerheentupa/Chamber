using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleObjectTest : MonoBehaviour, IGrapple
{
    Rigidbody rig;
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Grab(Vector3 direction) {
        print("Object " + gameObject.name + " grabbed, with vector: " + direction.x + ", " + direction.y + ", " + direction.z);
        rig.AddForce(direction, ForceMode.Impulse);
    }
}
