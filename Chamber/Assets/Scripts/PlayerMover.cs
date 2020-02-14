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
    public float movementSpeedMultiplier = 2;
    public float walkMomentum = 125;
    public float walkMomentumMax = 100;
    public float sprintMomentum = 175;
    public float sprintMultiplier = 2;
    public float momentumStop = 280;
    float sprintMomentumMax;
    public float maxVelocity = 2;
    Vector3 lastInput = Vector3.zero;
    bool sprinting = false;
    bool crouching = false;
    bool isMoving = false;
    public Animator cameraAnim;
    float normalFoV;
    float maxFoV;
    void Start() {
        rig = GetComponent<Rigidbody>();
        sprintMomentumMax = walkMomentumMax * sprintMultiplier;
        normalFoV = Camera.main.fieldOfView;
        maxFoV = normalFoV + 15f;
    }

    void Update() {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Sprint") && !sprinting) {
            cameraAnim.Play("CameraSprintStart");
            sprinting = true;
            return;
        }
        if (Input.GetButtonDown("Sprint") && sprinting) {
            cameraAnim.Play("CameraSprintStop");
            sprinting = false;
            return;
        }
        if (Input.GetButtonDown("Crouch") && !crouching) {
            cameraAnim.Play("CameraCrouch");
            crouching = true;
            return;
        }
        if (Input.GetButtonDown("Crouch") && crouching) {
            cameraAnim.Play("CameraCrouchEnd");
            crouching = false;
            return;
        }
        if (momentum <= 0) {
            sprinting = false;
        }
        if (vertical != 0 || horizontal != 0) {
            isMoving = true;
            lastInput = transform.TransformDirection(new Vector3(horizontal, 0, vertical));
        } else isMoving = false;
        //print("Momentum: " + momentum + " Last input: " + lastInput.x + "X " + lastInput.z + "Z");
    }

    private void FixedUpdate() {
        Vector3 inputVector = new Vector3(horizontal, 0, vertical);
        inputVector = transform.TransformDirection(inputVector);
        if(isMoving && !sprinting) {
            WalkMomentum(inputVector);
        }
        if (isMoving && sprinting) {
            RunMomentum(inputVector);
        }
        Vector3 velocity = inputVector * momentum * Time.deltaTime * movementSpeedMultiplier;
        //velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        CheckInputDirectionHorizontal(inputVector, velocity);
        CheckInputDirectionVertical(inputVector, velocity);
        if (HasMomentum()) {
            rig.velocity = velocity;
        }
        if (inputVector.magnitude == 0 && HasMomentum()) {
            momentum -= Time.deltaTime * momentumStop;
            if (momentum < 0) {
                momentum = 0;
            }
        }
        if (HasMomentum() && !isMoving) {
            velocity = momentum * lastInput * Time.deltaTime * movementSpeedMultiplier;
            rig.velocity = velocity;
        }
        if (momentum > walkMomentumMax && Camera.main.fieldOfView < maxFoV) {
            Camera.main.fieldOfView = normalFoV + (1f * (momentum - walkMomentumMax) / 10);
            if (Camera.main.fieldOfView > maxFoV) {
                Camera.main.fieldOfView = maxFoV;
            }
        } else Camera.main.fieldOfView = normalFoV;
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

    void WalkMomentum(Vector3 inputVector) {
        if (inputVector.magnitude > 0 && momentum < walkMomentumMax) {
            momentum += Time.deltaTime * walkMomentum;
            if (momentum > walkMomentumMax) {
                momentum = walkMomentumMax;
            }
        }
        if (momentum > walkMomentumMax) {
            momentum -= Time.deltaTime * walkMomentum;
            if (momentum < 0) {
                momentum = 0;
            }
        }
    }

    void RunMomentum(Vector3 inputVector) {
        if (inputVector.magnitude > 0 && momentum < sprintMomentumMax) {
            momentum += Time.deltaTime * sprintMomentum;
            if (momentum > sprintMomentumMax) {
                momentum = sprintMomentumMax;
            }
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

    bool HasInput() {
        if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Vertical") > 0) {
            print("input!");
            return true;
        } else return false;
    }

    bool HasMomentum() {
        if (momentum > 0) {
            return true;
        } else return false;
    }
}
