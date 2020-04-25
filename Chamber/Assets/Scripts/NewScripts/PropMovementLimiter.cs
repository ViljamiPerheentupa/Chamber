using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMovementLimiter : MonoBehaviour
{
    Transform upperLimit;
    Transform lowerLimit;
    Vector3 targetDestination;

    public LayerMask layerMask;
    RaycastHit hit;
    float radius;

    bool reachedLower = false;
    bool reachedUpper = false;
    public bool lockOnUp = false;

    Rigidbody rig;
    void Start()
    {
        targetDestination = Vector3.zero;
        upperLimit = transform.parent.Find("UpperLimit").transform;
        lowerLimit = transform.parent.Find("LowerLimit").transform;
        radius = GetComponent<Collider>().bounds.size.y / 2;
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate() {
        if (HasDestination()) {
            rig.position = targetDestination;
            rig.velocity = Vector3.zero;
            targetDestination = Vector3.zero;
            if (reachedLower) {
                rig.useGravity = false;
            } else if (reachedUpper && lockOnUp) {
                rig.useGravity = false;
                rig.isKinematic = true;
            }
        } else {
            if (Physics.Raycast(rig.position, rig.velocity, out hit, rig.velocity.magnitude * Time.fixedDeltaTime, layerMask)) {
                if (hit.collider.transform == upperLimit) {
                    print("Hit Upper limit");
                    targetDestination = upperLimit.position + (Vector3.down * radius);
                    reachedUpper = true;
                }
                if (hit.collider.transform == lowerLimit) {
                    print("Hit Lower limit");
                    targetDestination = lowerLimit.position + (Vector3.up * radius);
                    reachedLower = true;
                }
            }
            else if (Physics.CheckSphere(rig.position, radius, layerMask)) {
                if (Vector3.Distance(rig.position, upperLimit.position) < Vector3.Distance(rig.position, lowerLimit.position)) {
                    reachedUpper = true;
                } else reachedLower = true;
            }
            if (reachedUpper || reachedLower) {
                if (!Physics.CheckSphere(rig.position, radius, layerMask)) {
                    reachedLower = false;
                    reachedUpper = false;
                    rig.useGravity = true;
                }
            }
        }
    }

    bool HasDestination() {
        return targetDestination != Vector3.zero;
    }
}
