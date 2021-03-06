﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLifting : MonoBehaviour {
    public Gun gun;
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
    private Liftable prop;
    private Rigidbody rigid;

    void Start() {
        gun = GetComponent<Gun>();
    }

    public void StartReset() {
        prop = null;
        rigid = null;
    }

    void OnFire(InputValue value) {
        if (!GameManager.Instance.isPaused && !GetComponent<PlayerHealth>().isDead && prop) {
            rigid.AddForce(Camera.main.transform.forward * throwStrength, ForceMode.Impulse);
            Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
            if (gun)
                gun.SetHolstering(false);
            prop = null;
            rigid = null;
        }
    }

    void OnInteract(InputValue value) {
        if (!GameManager.Instance.isPaused && !GetComponent<PlayerHealth>().isDead) {
            if (prop) {
                Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
                if (gun)
                    gun.SetHolstering(false);
                prop = null;
                rigid = null;
            }
            else {
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxLiftDistance, rayTraceLayerMask)) {
                    if (hit.collider) {
                        Liftable liftable = hit.collider.GetComponent<Liftable>();
                        if (liftable && liftable.isLiftable) {
                            if (gun)
                                gun.SetHolstering(true);
                            prop = liftable;
                            rigid = prop.GetComponent<Rigidbody>();
                            liftRotation = Quaternion.Euler(prop.transform.rotation.eulerAngles - characterTransform.rotation.eulerAngles);
                            Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), hit.collider, true);
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        if (GetComponent<PlayerHealth>().isDead) {
            prop = null;
            rigid = null;
            return;
        }

        if (prop) {
            Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * prop.liftDistance;
            Vector3 propMovementVector = targetPos - prop.transform.position;
            if (propMovementVector.magnitude < abandonDistance) {
                rigid.velocity = liftForce * propMovementVector;
                Quaternion targetRotation = characterTransform.rotation * liftRotation;
                rigid.MoveRotation(Quaternion.RotateTowards(rigid.rotation, targetRotation, rotateSpeed * Time.deltaTime));
            }
            else {
                // Prop is too far, drop it
                Physics.IgnoreCollision(characterTransform.GetComponent<Collider>(), prop.GetComponent<Collider>(), false);
                gun.SetHolstering(false);
                prop = null;
                rigid = null;
            }
        }
    }
}
