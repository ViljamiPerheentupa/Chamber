using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Private
    private bool isSprinting;
    private Vector2 moveAxis;
    private float weaponBobSpeedChange;
    private float weaponBobAmountChange;
    public float weaponBobFactorChangeSpeed = 0.2f;
    private float weaponBobSpeed = 0.0f;
    private float weaponBobAmount = 0.0f;
    private float weaponBobTime = 0.0f;
    public Vector2 weaponBobStretch = new Vector2(2.0f, 1.0f);
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
    public float couchBobSpeed = 2.0f;
    public float runBobSpeed = 1.0f;
    public float sprintBobSpeed = 2.0f;
    public float couchBobAmount = 2.0f;
    public float runBobAmount = 1.0f;
    public float sprintBobAmount = 2.0f;

    private float previousVelocityY;
    private float endOfJumpTime = 0.0f;
    private bool needFootstep = false;
    private bool forwardPressed;
    private bool backwardPressed;
    private bool leftPressed;
    private bool rightPressed;

    Rigidbody rigidBody;
    Collider c_collider;

    void Start() {
        c_collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    public void StartReset() {
        rigidBody.velocity = new Vector3();
        weaponSway = new Vector3();
        isCrouching = false;
        startCrouchTime = 0;
    }

    bool isGrounded() {
        return Physics.CheckCapsule(c_collider.bounds.center,new Vector3(c_collider.bounds.center.x,c_collider.bounds.min.y-0.1f,c_collider.bounds.center.z),0.18f, floorMask);
    }

    bool CanStand() {
        Vector3 topOfHead = transform.position;
        float distance = standCapsuleHeight - crouchCapsuleHeight;
        return !Physics.Raycast(topOfHead, Vector3.up, distance, floorMask); 
    }
    
    void CalculateBob() {
        /*
        weaponBobTime += weaponBobSpeed * Time.deltaTime;
        float t = weaponBobTime;
        weaponSwayKick = Vector2.SmoothDamp(weaponSwayKick, new Vector2(0,0), ref weaponSwayKickVelocity, weaponSwayRecoverSpeed);
        weaponSway = Vector3.SmoothDamp(weaponSway, new Vector3(0,0,0), ref weaponSwayVelocity, weaponSwayRecoverSpeed);
        weaponSway.x -= (weaponSwayKick.y + Input.GetAxisRaw("Mouse Y") * weaponSwayAmount.y) * Time.deltaTime;
        weaponSway.y -= (weaponSwayKick.x + Input.GetAxisRaw("Mouse X") * weaponSwayAmount.x) * Time.deltaTime;

        float sint = Mathf.Sin(2 * t);
        if (sint < -0.9) {
            if (needFootstep) {
                PlayFootstep();
                needFootstep = false;
            }
        }
        else {
            needFootstep = true;
        }

        Vector3 weaponBob = weaponBobAmount * new Vector3(weaponBobStretch.y * sint, weaponBobStretch.x * Mathf.Sin(t + Mathf.PI / 2.0f), 0);
        gunTransform.localEulerAngles = weaponSway + weaponBob;
        */
    }

    void OnMove(InputValue value) {
        moveAxis = value.Get<Vector2>();
    }

    void OnForward(InputValue value) {
        forwardPressed = value.isPressed;
    }

    void OnBackward(InputValue value) {
        backwardPressed = value.isPressed;
    }

    void OnLeft(InputValue value) {
        leftPressed = value.isPressed;
    }

    void OnRight(InputValue value) {
        rightPressed = value.isPressed;
    }

    void OnLook(InputValue value) {
        //look = value.Get<Vector2>();
        cameraTransform.GetComponent<MouseLook>().lookAxis = value.Get<Vector2>();
    }

    void OnJump() {
        if (isGrounded()) {
            weaponSwayKick.y -= jumpSway;
            rigidBody.AddForce(new Vector3(0, jumpImpulse, 0), ForceMode.Impulse);
            endOfJumpTime = Time.time + jumpContinuationTime;
        }
    }

    void OnSprint(InputValue value) {
        isSprinting = value.isPressed;
    }

    void OnCrouch(InputValue value) {
        float t = crouchDelay - (Time.time - startCrouchTime);
        t = Mathf.Clamp(t, 0.0f, crouchDelay);
        if (value.isPressed) {
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
    
    void Update() {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().paused || GetComponent<PlayerHealth>().isDead || GetComponent<GunMagnet>().isPulling) {
            return;
        }

        CalculateBob();
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

    public void PlayFootstep() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/PFootstep");
    }

    void FixedUpdate() {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().paused || GetComponent<GunMagnet>().isPulling) {
            return;
        }

        if (GetComponent<PlayerHealth>().isDead) {
            rigidBody.velocity = new Vector3();
        }

        moveAxis.x = (leftPressed ? -1 : 0) + (rightPressed ? 1 : 0);
        moveAxis.y = (forwardPressed ? 1 : 0) + (backwardPressed ? -1 : 0);
        
        HandleCrouch();

        float moveSpeed = 0.0f;
        float targetBobSpeed = 0.0f;
        float targetBobAmount = 0.0f;

        Vector3 velocity;
        Vector3 newVelocity = new Vector3();

        
        if (previousVelocityY < -0.05f) {
            if (rigidBody.velocity.y > -0.05f) {
                float vol = Mathf.Clamp(previousVelocityY * previousVelocityY / 128.0f, 0.0f, 3.0f);
                if (vol > 0.2f) {
                    FMODUnity.RuntimeManager.PlayOneShotAtVolume("event:/SFX/Pland", vol);
                }
                weaponSwayKick.y += landSwayMultiplier * -previousVelocityY;
            }
        }

        if (isGrounded()) {
            velocity = rigidBody.velocity;

            velocity.x *= maintainSpeedCoefficient;
            velocity.z *= maintainSpeedCoefficient;

            if (isCrouching) {
                moveSpeed = crouchMoveSpeed;
                targetBobSpeed = couchBobSpeed;
                targetBobAmount = couchBobAmount;
            }
            else if (isSprinting) {
                moveSpeed = sprintSpeed;
                targetBobSpeed = sprintBobSpeed;
                targetBobAmount = sprintBobAmount;
            }
            else {
                moveSpeed = runSpeed;
                targetBobSpeed = runBobSpeed;
                targetBobSpeed = runBobSpeed;
                targetBobAmount = runBobAmount;
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

        if (moveAxis.x == 0f && moveAxis.y == 0f) {
            targetBobSpeed = 0f;
            targetBobAmount = 0f;
        }

        newVelocity = transform.forward * moveAxis.y + transform.right * moveAxis.x;
        newVelocity *= moveSpeed;

        float speed = Vector3.Magnitude(newVelocity);
        speed = Mathf.Min(speed, maxSpeed);
        weaponBobSpeed = Mathf.SmoothDamp(weaponBobSpeed, targetBobSpeed, ref weaponBobSpeedChange, weaponBobFactorChangeSpeed);
        weaponBobAmount = Mathf.SmoothDamp(weaponBobAmount, targetBobAmount, ref weaponBobAmountChange, weaponBobFactorChangeSpeed);

        velocity += Vector3.Normalize(newVelocity) * speed;

        previousVelocityY = velocity.y;

        rigidBody.velocity = velocity;
    }
}
