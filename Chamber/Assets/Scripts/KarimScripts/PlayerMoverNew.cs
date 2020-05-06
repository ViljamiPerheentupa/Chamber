using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMoverNew : MonoBehaviour {
    public float crouchMoveSpeed = 0.5f;
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
    public float cameraHeight = -0.25f;
    public float standCapsuleHeight = 2.0f;
    public float crouchCapsuleHeight = 1.0f;
    public float startCrouchTime = 0.0f;
    public float crouchDelay = 0.5f;
    public bool isCrouching = false;
    public AnimationCurve crouchCurve;
    public Transform cameraTransform;
    public Transform gunTransform;
    private float weaponBobFactorChange;
    public float weaponBobFactorChangeSpeed = 0.2f;
    private float weaponBobFactor = 0.0f;
    private float weaponBobTime = 0.0f;
    public float weaponBobSpeed = 4f;
    public Vector2 weaponBobAmount = new Vector2(2.0f, 2.0f);
    public Vector2 weaponSwayAmount;
    private Vector2 weaponSwayKick;
    private Vector2 weaponSwayKickVelocity;
    public float weaponSwayRecoverSpeed = 0.2f;
    private Vector3 weaponSway;
    private Vector3 weaponSwayVelocity;
    public float standToCrouchWeaponSway = 80.0f;
    public float crouchToStandWeaponSway = 80.0f;
    public float jumpSway = 100.0f;
    public float landSwayMultiplier = 10.0f;

    private float previousVelocityY;
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

    bool CanStand() {
        Vector3 topOfHead = transform.position;
        float distance = 1.0f;
        return !Physics.Raycast(topOfHead, Vector3.up, distance, floorMask); 
    }
    
    void CalculateBob() {
        weaponBobTime += weaponBobFactor * Time.deltaTime;
        float t = weaponBobTime * weaponBobSpeed;
        weaponSwayKick = Vector2.SmoothDamp(weaponSwayKick, new Vector2(0,0), ref weaponSwayKickVelocity, weaponSwayRecoverSpeed);
        weaponSway = Vector3.SmoothDamp(weaponSway, new Vector3(0,0,0), ref weaponSwayVelocity, weaponSwayRecoverSpeed);
        weaponSway.x -= (weaponSwayKick.y + Input.GetAxisRaw("Mouse Y") * weaponSwayAmount.y) * Time.deltaTime;
        weaponSway.y -= (weaponSwayKick.x + Input.GetAxisRaw("Mouse X") * weaponSwayAmount.x) * Time.deltaTime;
        Vector3 weaponBob = weaponBobFactor * new Vector3(weaponBobAmount.y * Mathf.Sin(2 * t), weaponBobAmount.x * Mathf.Sin(t + Mathf.PI / 2.0f), 0);
        gunTransform.localEulerAngles = weaponSway + weaponBob;
    }
    
    void Update() {
        CalculateBob();

        if (Input.GetButtonDown("Jump")) {
            if (isGrounded()) {
                weaponSwayKick.y -= jumpSway;
                rigidBody.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
                endOfJumpTime = Time.time + jumpContinuationTime;
            }
        }
        
        float t = crouchDelay - (Time.time - startCrouchTime);
        t = Mathf.Clamp(t, 0.0f, crouchDelay);
        if (Input.GetButton("Crouch")) {
            if (!isCrouching) {
                weaponSwayKick.y += standToCrouchWeaponSway;
                isCrouching = true;
                startCrouchTime = Time.time - t;
            }
        }
        else {
            if (isCrouching) {
                if (CanStand()) {
                    weaponSwayKick.y -= crouchToStandWeaponSway;
                    isCrouching = false;
                    startCrouchTime = Time.time - t;
                }
            }
        }
    }

    void HandleCrouch() {
        /*cameraHeight = -0.25f;
        public float standCapsuleHeight = 2.0f;
        public float crouchCapsuleHeight = 1.0f;
        public float startCrouchTime = 0.0f;
        public float crouchDelay;

        float currentCamHeight*/

        float t = (Time.time - startCrouchTime) / crouchDelay;
        if (isCrouching) t = 1 - t;

        t = crouchCurve.Evaluate(t);

        float currentCapsuleHeight = Mathf.Lerp(crouchCapsuleHeight, standCapsuleHeight, t);
        float currentCapsulePosition = (currentCapsuleHeight - standCapsuleHeight) / 2.0f;
        float currentCameraHeight = currentCapsulePosition + currentCapsuleHeight/2.0f + cameraHeight;
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.height = currentCapsuleHeight;
        capsuleCollider.center = new Vector3(0.0f, currentCapsulePosition, 0.0f);
        cameraTransform.localPosition = new Vector3(0.0f, currentCameraHeight, 0.0f);
    }

    void FixedUpdate() {
        HandleCrouch();

        bool isSprinting = Input.GetButton("Sprint");
        float moveSpeed = 0.0f;

        Vector3 velocity;
        Vector3 newVelocity = new Vector3();

        
        if (previousVelocityY < -0.05f) {
            if (rigidBody.velocity.y > -0.05f) {
                weaponSwayKick.y += landSwayMultiplier * -previousVelocityY;
            }
        }

        if (isGrounded()) {
            velocity = rigidBody.velocity;

            velocity.x *= maintainSpeedCoefficient;
            velocity.z *= maintainSpeedCoefficient;

            if (isCrouching) {
                moveSpeed = crouchMoveSpeed;
            }
            else if (isSprinting) {
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
        weaponBobFactor = Mathf.SmoothDamp(weaponBobFactor, speed / sprintSpeed, ref weaponBobFactorChange, weaponBobFactorChangeSpeed);

        velocity += Vector3.Normalize(newVelocity) * speed;

        previousVelocityY = velocity.y;

        rigidBody.velocity = velocity;
    }
}
