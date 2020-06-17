using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMover : MonoBehaviour {
    public PlayerMoverData config;
    public Transform cameraTransform;
    public Transform gunTransform;

    #region Private Variables
    private bool isCrouchPressed = false;
    private bool isCrouching = false;
    private bool isSprinting;
    private Vector2 moveAxis;
    private float weaponBobSpeedChange;
    private float weaponBobAmountChange;
    private float weaponBobSpeed = 0.0f;
    private float weaponBobAmount = 0.0f;
    private float weaponBobTime = 0.0f;
    private Vector2 weaponSwayKick;
    private Vector2 weaponSwayKickVelocity;
    private Vector3 weaponSway;
    private Vector3 weaponSwayVelocity;
    private bool canExtendJump;
    private bool isJumpPressed;
    private float previousVelocityY;
    private float endOfJumpTime = 0.0f;
    private bool needFootstep = false;
    private bool isNoclipping;
    private float coyoteTimeEnd = 0f;
    private bool canLand = false;
    #endregion

    Rigidbody rigidBody;
    Collider c_collider;
    Animator animator;

    void Start() {
        c_collider = GetComponent<Collider>();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void StartReset() {
        rigidBody.velocity = new Vector3();
        weaponSway = new Vector3();
        isCrouching = false;
        config.startCrouchTime = 0;
        SetNoclip(false);
    }

    bool isGrounded() {
        return Physics.CheckCapsule(c_collider.bounds.center,new Vector3(c_collider.bounds.center.x,c_collider.bounds.min.y-0.1f,c_collider.bounds.center.z),0.18f, config.floorMask);
    }

    bool CanStand() {
        Vector3 topOfHead = transform.position;
        float distance = config.standCapsuleHeight - config.crouchCapsuleHeight;
        return !Physics.Raycast(topOfHead, Vector3.up, distance, config.floorMask); 
    }
    
    void CalculateBob() {
        weaponBobTime += weaponBobSpeed * Time.deltaTime;
        float t = weaponBobTime;
        weaponSwayKick = Vector2.SmoothDamp(weaponSwayKick, new Vector2(0,0), ref weaponSwayKickVelocity, config.weaponSwayRecoverSpeed);
        weaponSway = Vector3.SmoothDamp(weaponSway, new Vector3(0,0,0), ref weaponSwayVelocity, config.weaponSwayRecoverSpeed);
        weaponSway.x -= (weaponSwayKick.y + Input.GetAxisRaw("Mouse Y") * config.weaponSwayAmount.y) * Time.deltaTime;
        weaponSway.y -= (weaponSwayKick.x + Input.GetAxisRaw("Mouse X") * config.weaponSwayAmount.x) * Time.deltaTime;

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

        Vector3 weaponBob = weaponBobAmount * new Vector3(config.weaponBobStretch.y * sint, config.weaponBobStretch.x * Mathf.Sin(t + Mathf.PI / 2.0f), 0);
        gunTransform.localEulerAngles = weaponSway + weaponBob;
    }

    public bool ToggleNoclip() {
        SetNoclip(!isNoclipping);
        return isNoclipping;
    }

    public void SetNoclip(bool status) {
        if (status) {
            if (!isNoclipping) {
                isNoclipping = true;
                GetComponent<Collider>().enabled = false;
                rigidBody.detectCollisions = false;
                rigidBody.useGravity = false;
            }
        }
        else {
            if (isNoclipping) {
                isNoclipping = false;
                GetComponent<Collider>().enabled = true;
                rigidBody.detectCollisions = true;
                rigidBody.useGravity = true;
            }
        }

        rigidBody.velocity = new Vector3();
    }

    void OnMove(InputValue value) {
        moveAxis = value.Get<Vector2>();
    }

    void OnLook(InputValue value) {
        cameraTransform.GetComponent<MouseLook>().lookAxis = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        isJumpPressed = value.isPressed;
        if (!GameManager.Instance.isPaused && !isNoclipping) {
            if (isJumpPressed) {
                Vector3 targetPosition, targetDirection;
                if (CanVault(out targetPosition, out targetDirection)) {
                    Vault(targetPosition, targetDirection);
                }
                else if (Time.time < coyoteTimeEnd) {
                    weaponSwayKick.y -= config.jumpSway;
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, config.jumpImpulse, rigidBody.velocity.z);
                    endOfJumpTime = Time.time + config.jumpContinuationTime;
                    canExtendJump = true;
                    coyoteTimeEnd = 0f;
                    canLand = false;
                }
            }
            else {
                canExtendJump = false;
            }
        }
    }

    void OnSprint(InputValue value) {
        isSprinting = value.isPressed;
    }

    void OnCrouch(InputValue value) {
        isCrouchPressed = value.isPressed;

        if (!GameManager.Instance.isPaused && !isNoclipping && isCrouchPressed && !isCrouching) {
            float t = config.crouchDelay - (Time.time - config.startCrouchTime);
            t = Mathf.Clamp(t, 0.0f, config.crouchDelay);
            weaponSwayKick.y += config.standToCrouchWeaponSway;
            isCrouching = true;
            config.startCrouchTime = Time.time - t;
        }
    }

    void HandleCrouch() {
        /*cameraHeight = -0.25f;
        public float standCapsuleHeight = 2.0f;
        public float crouchCapsuleHeight = 1.0f;
        public float startCrouchTime = 0.0f;
        public float crouchDelay;

        float currentCamHeight*/

        float t = (Time.time - config.startCrouchTime) / config.crouchDelay;
        if (isCrouching) t = 1 - t;
        t = config.crouchCurve.Evaluate(t);

        float currentCapsuleHeight = Mathf.Lerp(config.crouchCapsuleHeight, config.standCapsuleHeight, t);
        float currentCapsulePosition = (currentCapsuleHeight - config.standCapsuleHeight) / 2.0f;
        float currentCameraHeight = currentCapsulePosition + currentCapsuleHeight/2.0f + config.cameraHeight;
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.height = currentCapsuleHeight;
        capsuleCollider.center = new Vector3(0.0f, currentCapsulePosition, 0.0f);
        cameraTransform.localPosition = new Vector3(0.0f, currentCameraHeight, 0.0f);
    }

    public void PlayFootstep() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/PFootstep");
    }

    bool CanFindVaultWall(float y, out RaycastHit hitHorizontal) {
        return Physics.Raycast(transform.position + Vector3.up * y, transform.forward, out hitHorizontal, config.vaultWallDistance);
    }

    bool CanVault(out Vector3 finalPos, out Vector3 finalDirection) {
        float playerCenterHeight = 1.0f;
        float vaultLittleBitExtra = 0.05f;

        RaycastHit hitHorizontal = new RaycastHit();
        bool wallFound = false;

        foreach (float height in config.vaultHeightChecks) {
            if (CanFindVaultWall(height, out hitHorizontal)) {
                wallFound = true;
                break;
            }
        }

        if (wallFound) {
            finalDirection = -hitHorizontal.normal;
            Vector3 topWallCheck = hitHorizontal.point - hitHorizontal.normal * config.vaultWallStandDepth + Vector3.up * (config.vaultWallMaxHeight - playerCenterHeight);
            
            RaycastHit hitVertical;
            if (Physics.Raycast(topWallCheck, -Vector3.up, out hitVertical, config.vaultWallMaxHeight - config.vaultWallMinHeight)) {
                Vector3 playerTargetCenter = hitVertical.point + Vector3.up * (playerCenterHeight + vaultLittleBitExtra);
                float playerCapsuleRadius = ((CapsuleCollider)c_collider).radius;
                float playerCapsuleHeight = ((CapsuleCollider)c_collider).height;
                float playerHeightMinusRadii = playerCapsuleHeight - 2 * playerCapsuleRadius;
                Vector3 playerHMRUp = Vector3.up * playerHeightMinusRadii / 2.0f;
                if (!Physics.CheckCapsule(playerTargetCenter - playerHMRUp, playerTargetCenter + playerHMRUp, playerCapsuleRadius)) {
                    finalPos = playerTargetCenter;
                    return true;
                }
            }
        }

        finalDirection = new Vector3();
        finalPos = new Vector3();
        return false;
    }

    void Vault(Vector3 vaultTarget, Vector3 direction) {
        transform.position = vaultTarget;
        Quaternion lookAt = Quaternion.LookRotation(direction);
        Vector3 lookEuler = lookAt.eulerAngles;
        cameraTransform.GetComponent<MouseLook>().SetAngles(lookEuler.x, lookEuler.y);
    }

    void FixedUpdate() {
        if (GameManager.Instance.isPaused || GetComponent<GunMagnet>().isPulling) {
            return;
        }

        if (isJumpPressed) {
            Vector3 targetPosition, targetDirection;
            if (CanVault(out targetPosition, out targetDirection)) {
                Vault(targetPosition, targetDirection);
            }
        }

        if (isGrounded()) {
            if (canLand) {
                // If I'm on the ground
                coyoteTimeEnd = Mathf.Infinity;
            }
        }
        else {
            if (coyoteTimeEnd == Mathf.Infinity) {
                // If I just got ungrounded
                coyoteTimeEnd = Time.time + config.coyoteTimeDuration;
            }
            canLand = true;
        }

        if (GetComponent<PlayerHealth>().isDead) {
            rigidBody.velocity = new Vector3();
            return;
        }

        if (isNoclipping) {
            float flySpeed = isSprinting ? config.noclipSprintSpeed : config.noclipSpeed;
            transform.position += (cameraTransform.forward * moveAxis.y + cameraTransform.right * moveAxis.x) * flySpeed * Time.deltaTime;
            transform.position += (cameraTransform.up * ((isJumpPressed ? 1f : 0f) - (isCrouchPressed ? 1f : 0f))) * flySpeed * Time.deltaTime;
            return;
        }

        if (!isCrouchPressed) {
            if (isCrouching) {
                if (CanStand()) {
                    float t = config.crouchDelay - (Time.time - config.startCrouchTime);
                    t = Mathf.Clamp(t, 0.0f, config.crouchDelay);
                    
                    weaponSwayKick.y -= config.crouchToStandWeaponSway;
                    isCrouching = false;
                    config.startCrouchTime = Time.time - t;
                }
            }
        }

        CalculateBob();
        
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
                weaponSwayKick.y += config.landSwayMultiplier * -previousVelocityY;
            }
        }

        if (Time.time < coyoteTimeEnd) {
            velocity = rigidBody.velocity;

            velocity.x *= config.maintainSpeedCoefficient;
            velocity.z *= config.maintainSpeedCoefficient;

            if (isCrouching) {
                moveSpeed = config.crouchMoveSpeed;
                targetBobSpeed = config.couchBobSpeed;
                targetBobAmount = config.couchBobAmount;
            }
            else if (isSprinting) {
                moveSpeed = config.sprintSpeed;
                targetBobSpeed = config.sprintBobSpeed;
                targetBobAmount = config.sprintBobAmount;
            }
            else {
                moveSpeed = config.runSpeed;
                targetBobSpeed = config.runBobSpeed;
                targetBobSpeed = config.runBobSpeed;
                targetBobAmount = config.runBobAmount;
            }
        }
        else {
            velocity = rigidBody.velocity;

            velocity.x *= config.maintainAirSpeedCoefficient;
            velocity.z *= config.maintainAirSpeedCoefficient;

            moveSpeed = config.airSpeed;

            if (rigidBody.useGravity) {
                if (velocity.y < 0) {
                    velocity.y += Physics.gravity.y * (config.fallMultiplier - 1) * Time.deltaTime;
                }
                else if (isJumpPressed && canExtendJump && endOfJumpTime > Time.time) {
                    velocity.y -= Physics.gravity.y * (config.jumpContinuationForce) * Time.deltaTime;
                }
            }
        }

        if (moveAxis.x == 0f && moveAxis.y == 0f) {
            targetBobSpeed = 0f;
            targetBobAmount = 0f;
        }

        newVelocity = transform.forward * moveAxis.y + transform.right * moveAxis.x;
        newVelocity *= moveSpeed;

        float speed = Vector3.Magnitude(newVelocity);
        speed = Mathf.Min(speed, config.maxSpeed);
        weaponBobSpeed = Mathf.SmoothDamp(weaponBobSpeed, targetBobSpeed, ref weaponBobSpeedChange, config.weaponBobFactorChangeSpeed);
        weaponBobAmount = Mathf.SmoothDamp(weaponBobAmount, targetBobAmount, ref weaponBobAmountChange, config.weaponBobFactorChangeSpeed);

        velocity += Vector3.Normalize(newVelocity) * speed;

        previousVelocityY = velocity.y;

        rigidBody.velocity = velocity;
        
        bool free = true;
        if (Time.time < coyoteTimeEnd) {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(c_collider.bounds.center.x,c_collider.bounds.min.y+0.01f,c_collider.bounds.center.z), -Vector3.up, out hit, 0.05f)) {
                if (moveAxis.magnitude == 0f && Vector3.Dot(hit.normal, Vector3.up) < 0.99f && velocity.y <= 0f) {
                    free = false;
                    rigidBody.velocity = new Vector3();
                    rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                }
            }
        }

        if (free) {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void DisableAnimator() {
        animator.enabled = false;
    }
}
