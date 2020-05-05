using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMoverNew : MonoBehaviour {
    public float gravity = 9.81f;
    public float airSpeed = 2.0f;
    public float runSpeed = 3.0f;
    public float sprintSpeed = 8.0f;
    public float maxSpeed = 40.0f;
    public float jumpImpulse = 6.0f;
    public float jumpContinuationForce = 0.6f;
    public float jumpContinuationTime = 0.4f;
    public float fallMultiplier = 1.8f;
    public float maintainAirSpeedCoefficient = 0.98f;
    public float maintainSpeedCoefficient = 0.8f;
    public LayerMask floorMask = ~(1 << 9); 

    private float endOfJumpTime = 0.0f;

    Rigidbody rigidBody;
    Collider c_collider;

    void Start() {
        c_collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    bool isGrounded() {
        return Physics.CheckCapsule(c_collider.bounds.center,new Vector3(c_collider.bounds.center.x,c_collider.bounds.min.y-0.1f,c_collider.bounds.center.z),0.18f, floorMask);
    }
    
    void Update() {
        if (Input.GetButtonDown("Jump")) {
            if (isGrounded()) {
                rigidBody.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
                endOfJumpTime = Time.time + jumpContinuationTime;
            }
        }
    }

    void FixedUpdate() {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float moveSpeed = 0.0f;

        Vector3 velocity;
        Vector3 newVelocity = new Vector3();

        if (isGrounded()) {
            velocity = rigidBody.velocity;

            velocity.x *= maintainSpeedCoefficient;
            velocity.z *= maintainSpeedCoefficient;

            if (isSprinting) {
                moveSpeed = sprintSpeed;
            }
            else {
                moveSpeed = runSpeed;
            }
        }
        else {
            velocity = rigidBody.velocity;

            velocity.x *= maintainAirSpeedCoefficient;
            velocity.z *= maintainAirSpeedCoefficient;

            moveSpeed = airSpeed;

            if (velocity.y < 0) {
                velocity.y += Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (Input.GetButton("Jump") && endOfJumpTime > Time.time) {
                velocity.y -= Physics.gravity.y * (jumpContinuationForce) * Time.deltaTime;
            }
        }

        float vert = Input.GetAxisRaw("Vertical");
        float horz = Input.GetAxisRaw("Horizontal");

        newVelocity = transform.forward * vert + transform.right * horz;
        newVelocity *= moveSpeed;

        float speed = Vector3.Magnitude(newVelocity);
        speed = Mathf.Min(speed, maxSpeed);

        velocity += Vector3.Normalize(newVelocity) * speed;

        rigidBody.velocity = velocity;
    }
}
