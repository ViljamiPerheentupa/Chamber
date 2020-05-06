using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifting : MonoBehaviour {
    public GunContainer gunContainer;
    [Tooltip("How far away you can pick something up.")]
    public float maxLiftDistance = 4.0f;
    [Tooltip("How far away an object is from in front of you before you let go of it.")]
    public float abandonDistance = 5.0f;
    [Tooltip("How much force a lifted object uses to go to its position.")]
    public float liftForce = 50.0f;
    public float rotateSpeed  = 40.0f;
    public LayerMask rayTraceLayerMask;
    public Transform characterTransform;
    public float throwStrength = 10.0f;


    // Private
    private Quaternion liftRotation;
    private DynamicProp prop;
    private Rigidbody rigid;

    void Update() {
        if (Input.GetKeyDown("e")) {
            if (prop) {
                Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
                gunContainer.SetHolstering(false);
                prop = null;
                rigid = null;
            }
            else {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.forward, out hit, maxLiftDistance, rayTraceLayerMask)) {
                    if (hit.collider) {
                        DynamicProp liftable = hit.collider.GetComponent<DynamicProp>();
                        if (liftable && liftable.isLiftable) {
                            gunContainer.SetHolstering(true);
                            prop = liftable;
                            rigid = prop.GetComponent<Rigidbody>();
                            liftRotation = Quaternion.Euler(prop.transform.rotation.eulerAngles - characterTransform.rotation.eulerAngles);
                            Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), hit.collider, true);
                        }
                    }
                }
            }
        }
        else if (Input.GetButtonDown("Fire1")) {
            rigid.AddForce(transform.forward * throwStrength, ForceMode.Impulse);
            Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
            gunContainer.SetHolstering(false);
            prop = null;
            rigid = null;
        }
        else if (prop) {
            Vector3 targetPos = transform.position + transform.forward * prop.liftDistance;
            Vector3 propMovementVector = targetPos - prop.transform.position;
            if (propMovementVector.magnitude < abandonDistance) {
                rigid.velocity = liftForce * propMovementVector;
                Quaternion targetRotation = characterTransform.rotation * liftRotation;
                rigid.MoveRotation(Quaternion.RotateTowards(rigid.rotation, targetRotation, rotateSpeed * Time.deltaTime));
            }
            else {
                // Prop is too far, drop it
                Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
                gunContainer.SetHolstering(false);
                prop = null;
                rigid = null;
            }
        }
    }
}
