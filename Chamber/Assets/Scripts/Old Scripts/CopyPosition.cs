using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform target;
    public Rigidbody rig;
    public bool crouching = false;
    bool crouched = false;
    public float crouchDistance = 0.75f;
    float crouch = 0;
    public float crouchSpeed = 5;
    Vector3 offset;
    void FixedUpdate()
    {
        if (crouching && !crouched) {
            CameraCrouch();
        }
        if (crouched && !crouching) {
            EndCrouch();
        }
        rig.position = target.GetComponent<Rigidbody>().position + offset;
    }

    void CameraCrouch() {
        if (crouch > -crouchDistance) {
            GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<PlayerMover>().stateLocked = true;
            crouch -= Time.fixedDeltaTime * crouchSpeed;
            if (crouch <= -crouchDistance) {
                crouch = -crouchDistance;
            }
            offset = new Vector3(0, crouch, 0);
        } else {
            GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<PlayerMover>().stateLocked = false;
            crouched = true;
            crouch = -crouchDistance;
        }
    }

    void EndCrouch() {
        if (crouch < 0) {
            GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<PlayerMover>().stateLocked = true;
            crouch += Time.fixedDeltaTime * crouchSpeed;
            if (crouch >= 0) {
                crouch = 0;
            }
            offset = new Vector3(0, crouch, 0);
        } else {
            GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<PlayerMover>().stateLocked = false;
            crouched = false;
            crouch = 0;
        }
    }
}
