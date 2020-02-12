using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    Rigidbody rig;
    public float movementSpeed;
    //public float maxVelocityChange;
    float vertical;
    float horizontal;
    float momentum = 0;
    public float momentumMultiplier = 10;
    float maxMomentum = 1;
    public float maxVelocity = 2;
    Vector3 lastPosition = Vector3.zero;
    Vector3 positionChange = Vector3.zero;
    void Start() {
        rig = GetComponent<Rigidbody>();
    }

    void Update() {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        print(momentum);
    }

    private void FixedUpdate() {
        Vector3 inputVector = new Vector3(horizontal, 0, vertical);
        //inputVector = transform.TransformDirection(inputVector);
        if (inputVector.magnitude > 0 && momentum < maxMomentum) {
            momentum += Time.deltaTime * momentumMultiplier;
            if (momentum > maxMomentum) {
                momentum = maxMomentum;
            }
        }
        if (HasInput() && HasMomentum()) {
            momentum -= Time.deltaTime * momentumMultiplier;
            if (momentum < 0) {
                momentum = 0;
            }
        }
        Vector3 velocity = inputVector * movementSpeed * momentum;
        //velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        if (HasMomentum()) {
            lastPosition = rig.position;
            rig.position += velocity * Time.deltaTime;
            positionChange = rig.position - lastPosition;
        }
        if (HasMomentum() && !HasInput()) {
            rig.position += positionChange * movementSpeed * momentum;
        }

        //Vector3 inputVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //inputVelocity = transform.TransformDirection(inputVelocity);
        //Vector3 targetVelocity = inputVelocity * movementSpeed;

        //Vector3 velocity = rig.velocity;
        //Vector3 velocityChange = targetVelocity - velocity;
        //velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        //velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        //velocityChange.y = 0;
        //rig.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    bool HasInput() {
        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0) {
            return true;
        } else return false;
    }

    bool HasMomentum() {
        if (momentum > 0) {
            return true;
        } else return false;
    }
}
