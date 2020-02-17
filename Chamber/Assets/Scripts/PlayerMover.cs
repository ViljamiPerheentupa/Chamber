using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlayerState { Normal, Sprint, Slide, Crouch, Airborne, Empty };
[System.Serializable]
public struct MomentumState {
    public PlayerState state;
    public float momentumIncrease;
    public float momentumMax;
}

public class PlayerMover : MonoBehaviour {
    Rigidbody rig;
    public float movementSpeed;
    //public float maxVelocityChange;
    float vertical;
    float horizontal;
    float momentum = 0;
    public float movementSpeedMultiplier = 2;
    [Tooltip("Normal PlayerState attributes")]
    public MomentumState normal;
    [Tooltip("Sprint PlayerState attributes")]
    public MomentumState sprint;
    [Tooltip("Slide PlayerState attributes")]
    public MomentumState slide;
    [Tooltip("Crouch PlayerState attributes")]
    public MomentumState crouch;
    [Tooltip("Airborne PlayerState attributes")]
    public MomentumState airborne;
    float momentumMax;
    float momentumIncrease;
    public float momentumDecay = 160;
    public float momentumDecayStopped = 280;
    public float slideMomentumRequirement = 140;
    public float slideDuration = 0.5f;
    float slideTimer = 0;
    bool canSlide = true;
    float slideCDTimer = 0;
    public float slideCooldown = 0.5f;
    public float jumpHeight = 2f;
    [Tooltip("Total amount of momentum lost upon hitting the ground. 1 = 100%")]
    public float groundMomentumLoss = 0.5f;
    [Tooltip("Amount of time it takes to lose said momentum, in seconds")]
    public float groundMomentumTime = 0.5f;
    float groundMomentumLossTimer = 0;
    Vector3 lastInput = Vector3.zero;
    public bool sprinting = false;
    public bool sliding = false;
    public bool crouching = false;
    public bool jumped = false;
    public float inAirControl = 0.4f;
    bool canJump = true;
    bool isAirborne = false;
    bool isMoving = false;
    bool losingMomentum = false;
    float normalFoV;
    float maxFoV;
    float targetFoV;
    float fovMomentumDeadzone;
    public float fovChangeSpeed = 5;

    PlayerState currentState = PlayerState.Normal;
    PlayerState lastInputState = PlayerState.Empty;
    public bool stateLocked = false;

    Animator anim;

    Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundCheckMask;
    bool isInGround = false;
    bool checkGround = true;
    public float inGroundCheckDistance = 0.01f;
    bool wasAirborne = false;
    bool goingUp = false;
    Vector3 lastPosition;
    void Start() {
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        normalFoV = Camera.main.fieldOfView;
        maxFoV = normalFoV + 25f;
        fovMomentumDeadzone = normal.momentumMax;
        groundCheck = transform.Find("GroundSensor");
        lastPosition = rig.position;
        targetFoV = normalFoV;
    }

    void Update() {
        if (!sliding || isAirborne) {
            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        if (vertical != 0 || horizontal != 0) {
            isMoving = true;
            lastInput = transform.TransformDirection(new Vector3(horizontal, 0, vertical));
        } else isMoving = false;
        if (Input.GetButtonDown("Sprint") && !sprinting && isMoving) {
            lastInputState = PlayerState.Sprint;
            sprinting = true;
            return;
        }
        if (Input.GetButtonDown("Sprint") && sprinting) {
            lastInputState = PlayerState.Normal;
            return;
        }
        if (Input.GetButtonDown("Crouch") && !crouching && !isAirborne && !losingMomentum) {
            lastInputState = PlayerState.Crouch;
            return;
        }
        if (Input.GetButton("Crouch") && isAirborne) {
            lastInputState = PlayerState.Crouch;
        }
        if (Input.GetButtonDown("Crouch") && crouching && !losingMomentum) {
            lastInputState = PlayerState.Normal;
            return;
        }
        if (Input.GetButtonDown("Crouch") && sliding) {
            lastInputState = PlayerState.Normal;
            return;
        }
        if (Input.GetButtonDown("Jump") && !isAirborne) {
            lastInputState = PlayerState.Airborne;
            jumped = true;
            checkGround = false;
            return;
        }
        if (momentum <= 0 && currentState == PlayerState.Sprint) {
            lastInputState = PlayerState.Normal;
        }
    }

    private void LateUpdate() {
        if (sliding) {
            Slide();
        }
        if (!canSlide && !sliding) {
            slideCDTimer += Time.deltaTime;
            if (slideCDTimer >= slideCooldown) {
                slideCDTimer = 0;
                canSlide = true;
            }
        }
        if (!stateLocked || (isAirborne && lastInputState == PlayerState.Crouch) ) {
            CheckState();
        }
        print(currentState + ", Momentum: " + momentum + " Last Input State: " + lastInputState);
    }

    private void FixedUpdate() {
        if (Physics.Raycast(groundCheck.position + Vector3.down * groundCheckRadius, Vector3.up, 0.01f, groundCheckMask)) {
            Debug.DrawRay(groundCheck.position + Vector3.down * groundCheckRadius, Vector3.up * 0.01f, Color.red);
            isInGround = true;
            rig.position += Vector3.up * 0.01f;
            print("Stuck in geometry!");
        } else {
            isInGround = false;
            Debug.DrawRay(groundCheck.position + Vector3.down * groundCheckRadius, Vector3.up * 0.01f, Color.green);
        }


        if (isAirborne) {
            wasAirborne = true;
            AirborneCheck();
        }


        if (losingMomentum) {
            HitGround();
        } else Momentum();
        Vector3 inputVector = new Vector3(horizontal, 0, vertical);
        inputVector = transform.TransformDirection(inputVector);

        if (jumped && canJump) {
            rig.AddForce(new Vector3(0, Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y), 0), ForceMode.VelocityChange);
            canJump = false;
        }


        Vector3 velocity = inputVector * momentum * Time.fixedDeltaTime * movementSpeedMultiplier;
        CheckInputDirectionHorizontal(inputVector, velocity);
        CheckInputDirectionVertical(inputVector, velocity);
        if (HasMomentum() && isMoving && !sliding && !isAirborne) {
            rig.velocity = velocity;
        }
        if (sliding && !isAirborne) {
            velocity = momentum * lastInput * Time.fixedDeltaTime * movementSpeedMultiplier;
            rig.velocity = velocity;
        }
        if (HasMomentum() && !isMoving && !isAirborne) {
            velocity = momentum * lastInput * Time.fixedDeltaTime * movementSpeedMultiplier;
            rig.velocity = velocity;
        }
        if (!HasMomentum() && !isMoving && !isAirborne) {
            rig.velocity = Vector3.zero;
        }
        if (isAirborne) {
            rig.velocity = new Vector3(rig.velocity.x + (inputVector.x * inAirControl), rig.velocity.y, rig.velocity.z);
        }


        if (momentum > fovMomentumDeadzone && Camera.main.fieldOfView < maxFoV) {
            targetFoV = normalFoV + (1f * (momentum - fovMomentumDeadzone) / 10);
            if (targetFoV > maxFoV) {
                targetFoV = maxFoV;
            }
        }
        if (momentum <= fovMomentumDeadzone) {
            if (targetFoV > normalFoV) {
                targetFoV = Mathf.Lerp(targetFoV, normalFoV, Time.deltaTime * 2);
            } else targetFoV = normalFoV;
        }
        if (Camera.main.fieldOfView != targetFoV) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFoV, Time.fixedDeltaTime * fovChangeSpeed);
        }


        CheckGround(velocity);
        if (currentState == PlayerState.Airborne) {
            isAirborne = true;
        }
    }

    void Slide() {
        slideTimer += Time.deltaTime;
        if (slideTimer >= slideDuration * 0.75f) {
            momentumIncrease = 0;
        }
        if (slideTimer >= slideDuration) {
            stateLocked = false;
            sliding = false;
            if (lastInputState == PlayerState.Empty) {
                lastInputState = PlayerState.Crouch;
            }
            CheckState();
            slideTimer = 0;
            return;
        }
    }
    void Momentum() {
        if (currentState == PlayerState.Normal) {
            momentumMax = normal.momentumMax;
            momentumIncrease = normal.momentumIncrease;
        }
        if (currentState == PlayerState.Sprint) {
            momentumMax = sprint.momentumMax;
            momentumIncrease = sprint.momentumIncrease;
        }
        if (currentState == PlayerState.Crouch) {
            momentumMax = crouch.momentumMax;
            momentumIncrease = crouch.momentumIncrease;
        }
        if (currentState == PlayerState.Slide) {
            momentumMax = slide.momentumMax;
            momentumIncrease = slide.momentumIncrease;
            stateLocked = true;
        }
        if (currentState == PlayerState.Airborne) {
            momentumMax = airborne.momentumMax;
            momentumIncrease = airborne.momentumIncrease;
        }

        if (isAirborne && !goingUp && momentum < momentumMax) {
            momentum += momentumIncrease * Time.fixedDeltaTime;
            return;
        }
        if (isMoving && momentum < momentumMax && !isAirborne) {
            momentum += momentumIncrease * Time.fixedDeltaTime;
            if (currentState != PlayerState.Slide) {
                if (momentum > momentumMax) {
                    momentum = momentumMax;
                }
            }
            return;
        }
        if (isMoving && momentum > momentumMax && !isAirborne && !sliding) {
            momentum -= momentumDecay * Time.fixedDeltaTime;
            if (momentum < momentumMax) {
                momentum = momentumMax;
            }
            return;
        }
        if (!isMoving && momentum > 0 && !isAirborne && !sliding) {
            momentum -= momentumDecayStopped * Time.fixedDeltaTime;
            if (momentum < 0) {
                momentum = 0;
            }
            return;
        }
    }

    void CheckState() {
        if (lastInputState == PlayerState.Empty) {
            return;
        }
        if (lastInputState == PlayerState.Normal) {
            currentState = PlayerState.Normal;
            if (Camera.main.gameObject.GetComponent<CopyPosition>().crouching) {
                Camera.main.gameObject.GetComponent<CopyPosition>().crouching = false;
                anim.Play("BodyStandUp");
            }
            sprinting = false;
            crouching = false;
            sliding = false;
            lastInputState = PlayerState.Empty;
            return;
        }
        if (lastInputState == PlayerState.Sprint) {
            currentState = PlayerState.Sprint;
            if (Camera.main.gameObject.GetComponent<CopyPosition>().crouching) {
                Camera.main.gameObject.GetComponent<CopyPosition>().crouching = false;
                anim.Play("BodyStandUp");
            }
            sprinting = true;
            crouching = false;
            sliding = false;
            lastInputState = PlayerState.Empty;
            return;
        }
        if (lastInputState == PlayerState.Crouch) {
            if (!Camera.main.gameObject.GetComponent<CopyPosition>().crouching) {
                Camera.main.gameObject.GetComponent<CopyPosition>().crouching = true;
                anim.Play("BodyCrouch");
            }
            if (momentum >= slideMomentumRequirement && canSlide) {
                currentState = PlayerState.Slide;
                canSlide = false;
                sprinting = false;
                crouching = false;
                sliding = true;
                lastInputState = PlayerState.Empty;
                return;
            } else if (!sliding) {
                currentState = PlayerState.Crouch;
                sprinting = false;
                sliding = false;
                crouching = true;
                lastInputState = PlayerState.Empty;
                return;
            }
        }
        if (lastInputState == PlayerState.Airborne) {
            currentState = PlayerState.Airborne;
            isAirborne = true;
            if (jumped) {
                crouching = false;
                sliding = false;
            }
            lastInputState = PlayerState.Empty;
            return;
        }
    }

    void AirborneCheck() {
        if (lastPosition.y < rig.position.y) {
            goingUp = true;
        } else goingUp = false;
        lastPosition = rig.position;
    }

    void CheckGround(Vector3 velocity) {
        if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundCheckMask) && checkGround) {
            isAirborne = false;
            rig.useGravity = false;
            if (wasAirborne) {
                //print("Hit ground");
                if (jumped) {
                    jumped = false;
                }
                //canJump = true;
                stateLocked = false;
                rig.velocity = new Vector3(velocity.x, 0, velocity.z);
                wasAirborne = false;
                losingMomentum = true;
                if (crouching && momentum >= slideMomentumRequirement && canSlide) {
                    crouching = false;
                    sliding = true;
                    currentState = PlayerState.Slide;
                    canSlide = false;
                    sprinting = false;
                    return;
                }
                if (lastInputState == PlayerState.Crouch) {
                    return;
                }
                if (lastInputState == PlayerState.Empty && !crouching && (!sprinting || lastInputState != PlayerState.Sprint)) {
                    lastInputState = PlayerState.Normal;
                }
                if (sprinting || lastInputState == PlayerState.Sprint) {
                    lastInputState = PlayerState.Sprint;
                }
                if (lastInputState == PlayerState.Empty && crouching) {
                    lastInputState = PlayerState.Crouch;
                }
            }
            return;
        } else {
            if (!checkGround && !Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundCheckMask)) {
                checkGround = true;
            }
            currentState = PlayerState.Airborne;
            canJump = false;
            rig.useGravity = true;
            stateLocked = true;
            return;
        }
    }

    void HitGround() {
        if (Input.GetButton("Crouch") && canSlide) {
            sliding = true;
            canJump = true;
            canSlide = false;
            crouching = false;
            sprinting = false;
            currentState = PlayerState.Slide;
            losingMomentum = false;
            return;
        }
        if (Input.GetButtonDown("Crouch") && !canSlide && crouching) {
            lastInputState = PlayerState.Normal;
        }
        if (!sliding && (lastInputState != PlayerState.Crouch || currentState != PlayerState.Slide)) {
            groundMomentumLossTimer += Time.fixedDeltaTime;
            if (groundMomentumLossTimer >= groundMomentumTime) {
                momentum -= momentum * groundMomentumLoss;
                groundMomentumLossTimer = 0;
                canJump = true;
                losingMomentum = false;
            }
        } else {
            losingMomentum = false;
            canJump = true;
            lastInputState = PlayerState.Crouch;
        }
    }

    void CheckInputDirectionHorizontal(Vector3 vector, Vector3 velocity) {
        if (vector.x > 0) {
            if (lastInput.x < 0) {
                rig.velocity = Vector3.zero;
                return;
            }
        }
        if (vector.x < 0) {
            if (lastInput.x > 0) {
                rig.velocity = Vector3.zero;
                return;
            }
        }
    }

    void CheckInputDirectionVertical(Vector3 vector, Vector3 velocity) {
        if (vector.z > 0) {
            if (lastInput.z < 0) {
                rig.velocity = Vector3.zero;
                return;
            }
        }
        if (vector.z < 0) {
            if (lastInput.z > 0) {
                rig.velocity = Vector3.zero;
                return;
            }
        }
    }

    bool HasMomentum() {
        if (momentum > 0) {
            return true;
        } else return false;
    }
}
