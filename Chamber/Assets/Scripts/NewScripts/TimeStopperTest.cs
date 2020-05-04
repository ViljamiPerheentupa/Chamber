using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TimeStopperTest : MonoBehaviour, IProp {

    public enum TimeLockType { Prop, Animator, Agent, Broken }
    TimeLockType type;
    bool hasAnimator;

    public bool timeLocked = false;
    [Tooltip("If the GameObject has a unique TimeLock duration, check this.")]
    public bool overrideDuration = false;
    [Tooltip("The unique duration. Won't do anything if the above box isn't checked.")]
    public float tlDuration = 5;
    [Tooltip("The magnitude of the 'glitch' effect during the TimeLock.")]
    public float tlEffectMagnitude = 0.25f;
    [Tooltip("The amount of slow applied. 0.01 = 1% of the normal speed.")]
    public float tlSlowAmount = 0.025f;
    float tlTimer = 0;

    Vector3 lockedDirection;
    Vector3 lockedRotation;
    float lockedAnimSpeed;
    bool hadGravity;
    bool wasStopped;

    Vector3 origPos;
    Quaternion origRot;

    Rigidbody rig;
    NavMeshAgent agent;
    Animator anim;
    GameObject tlGraphic;

    [Tooltip("For debugging purposes. The amount of force applied to the GameObject when the debug commands are used.")]
    public float impulseMagnitude = 5;

    void Start()
    {
        if (!overrideDuration) {    //If there is no override, use the default value
            tlDuration = 5;
        }
        type = TimeLockType.Broken;     //Get necessary values and components
        CheckType();
        GetNecessaryComponents(type);
        if (type == TimeLockType.Prop) {
            rig = GetComponent<Rigidbody>();
            origPos = rig.position;
            origRot = rig.rotation;
        }
    }

    void Update()
    {
        if (type == TimeLockType.Prop) {    //Just part of the debugging aspect
            DebugTypeProp();
        }

    }

    private void LateUpdate() {
        if (timeLocked) { //Put the actual effects into play
            TimeLockDuration();
            TimeLockEffect();
        }
    }

    void DebugTypeProp() { //Debug code used to make a prop move with physics, so I could see how the effect was working out
        if (Input.GetKeyDown(KeyCode.G)) {
            rig.useGravity = rig.useGravity ? false : true;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            SendImpulse(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            SendImpulse(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            SendImpulse(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            rig.velocity = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            rig.angularVelocity = Vector3.right * impulseMagnitude;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ResetObject();
        }
    }

    void SendImpulse(Vector3 direction) {
        PropForce(direction * impulseMagnitude, ForceMode.Impulse);
    }

    void TimeLockEffect() {     //The graphical effect of the TimeLock
        if (tlGraphic != null) {
            var effect = (tlTimer / tlDuration) * tlEffectMagnitude;
            var x = Random.Range(-effect, effect);      //Randomizing the "shakiness" of the effect
            var y = Random.Range(-effect, effect);
            var z = Random.Range(-effect, effect);
            tlGraphic.transform.localPosition = new Vector3(x, y, z);
        } else print("No graphic was found for TimeLock effect");
    }

    void TimeLockDuration() {       //This function counts down the time of the effect
        tlTimer += Time.deltaTime;
        if (type == TimeLockType.Prop) {
            SlowedPhysics();        //The slowed down physics are applied while the duration is still counting down
        }
        if (tlTimer >= tlDuration) {
            tlTimer = 0;
            timeLocked = false;
            ReleaseTimeLock();
        }
    }

    void SlowedPhysics() {      //How the physical rigidbodies act during the slow
        rig.velocity = lockedDirection * tlSlowAmount;
        rig.angularVelocity = lockedRotation * tlSlowAmount;
    }

    public void TimeLock() {        //Applying the TimeLock effect
        if (!timeLocked) {      //Making sure the TimeLock isn't applied on top of an earlier TimeLock
            if (type == TimeLockType.Broken) {
                print("Object's TimeLockType is Broken");       //Something went wrong when fetching the correct components
            } else {
                timeLocked = true;
                if (type == TimeLockType.Prop) {        //What effects happen if the GameObject in question is a prop
                    lockedDirection = rig.velocity;
                    lockedRotation = rig.angularVelocity;
                    print("Prop locked - Locked velocity vector: " + lockedDirection.x + ", " + lockedDirection.y + ", " + lockedDirection.z);
                    //rig.velocity = Vector3.zero;
                    //rig.angularVelocity = Vector3.zero;
                    hadGravity = rig.useGravity;
                    rig.useGravity = false;
                }
                if (type == TimeLockType.Animator || hasAnimator) {     //What happens if the GameObject is only an animator, or if it has an animator attached
                    lockedAnimSpeed = anim.speed;
                    anim.speed = tlSlowAmount;
                    if (tlGraphic.GetComponent<Animator>() != null) {
                        tlGraphic.GetComponent<Animator>().speed = 0;
                    }
                }
                if (type == TimeLockType.Agent) {       //What happens if the GameObject is an agent of the NavMesh
                    wasStopped = agent.isStopped;
                    agent.isStopped = true;
                }
            }
            if (tlGraphic != null) {        //Enable the TimeLock graphical effect
                tlGraphic.SetActive(true);
            }
        } else tlTimer = 0;     //Reset the TimeLock duration if a new one is applied before the old one ended

    }

    public void ReleaseTimeLock() {         //Ending the TimeLock
        if (type == TimeLockType.Prop) {
            print("Released prop");
            rig.velocity = lockedDirection;
            rig.angularVelocity = lockedRotation;
            rig.useGravity = hadGravity;
        }
        if (type == TimeLockType.Animator || hasAnimator) {
            anim.speed = lockedAnimSpeed;
            if (tlGraphic.GetComponent<Animator>() != null) {
                tlGraphic.GetComponent<Animator>().speed = lockedAnimSpeed;
            }
        }
        if (type == TimeLockType.Agent){
            agent.isStopped = wasStopped;
        }
        if (tlGraphic != null) {
            tlGraphic.SetActive(false);
        }
    }

    void ResetObject() {        //Debugging purposes
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        rig.position = origPos;
        rig.rotation = origRot;
    }

    void GetNecessaryComponents(TimeLockType tlt) {     //Fetching the correct components for the effect to work
        if (tlt == TimeLockType.Prop) {
            rig = GetComponent<Rigidbody>();
            if (rig.mass > 1 && !overrideDuration) {
                tlDuration -= rig.mass;     //Calculating the correct TimeLock duration, if no overwrite duration was given
            }                               //The calculation works off of a Rigidbody's Mass
        }
        if (tlt == TimeLockType.Agent) {
            agent = GetComponent<NavMeshAgent>();
        }
        if (tlt == TimeLockType.Animator || hasAnimator) {
            if (GetComponent<Animator>() != null) {
                anim = GetComponent<Animator>();
            } else anim = GetComponentInChildren<Animator>();
        }
        if (transform.Find("TimeLockGraphic") != null) {        //Checking that there's the proper TimeLock graphic in place
            tlGraphic = transform.Find("TimeLockGraphic").gameObject;
            tlGraphic.SetActive(false);
        } else Debug.LogError("No TimeLock effect graphic found on object " + gameObject.name + "!");       //If there isn't, print out an error but otherwise do nothing
    }

    void CheckType() {      //Checking the type of the GameObject, so it doesn't have to be manually be set
        if (gameObject.layer == LayerMask.NameToLayer("Props")) {
            if (GetComponent<Rigidbody>() != null) {
                type = TimeLockType.Prop;
            } else type = TimeLockType.Broken;         //If it isn't one of the three types, it's broken so as to not do anything
        }
        if (gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            if (GetComponent<NavMeshAgent>() != null) {
                type = TimeLockType.Agent;
            } else type = TimeLockType.Broken;
        }
        if (GetComponent<Animator>() != null || GetComponentInChildren<Animator>() != null) {
            if (type == TimeLockType.Prop || type == TimeLockType.Agent) {
                hasAnimator = true;
            } else type = TimeLockType.Animator;
            return;
        }
    }

    public void PropForce(Vector3 force, ForceMode forceMode) {     //The custom interface to add velocity to props, so it can work properly during a TimeLock
        if (timeLocked) {
            lockedDirection += force;
        } else rig.AddForce(force, forceMode);
    }

    public void PropExplosiveForce(Vector3 location, float force, float radius) {       //The same, but with the AddExplosionForce function
        if (timeLocked) {
            // TODO: Calculate "explosion" force and add it to lockedDirection
        } else rig.AddExplosionForce(force, location, radius);
    }
}
