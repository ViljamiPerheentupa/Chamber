using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    Rigidbody rig;
    public float movementSpeed;
    public float maxVelocityChange;
    void Start() {
        rig = GetComponent<Rigidbody>();
    }

    void Update() {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        print("Horizontal: " + )
    }

    private void LateUpdate() {
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
}
