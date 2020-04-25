using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TimeStopperTest : MonoBehaviour, ITimeLock
{
    public enum TimeLockType { Prop, Animator, Agent, Broken }
    public TimeLockType type;
    public bool hasAnimator;

    public bool timeLocked = false;
    bool overrideDuration = false;
    public float tlDuration = 5;
    public float tlEffectMagnitude = 0.25f;
    float tlTimer = 0;

    Vector3 lockedDirection;
    Vector3 lockedRotation;
    float lockedAnimSpeed;
    bool hadGravity;
    bool wasKinematic;
    bool wasStopped;

    Vector3 origPos;
    Quaternion origRot;

    Rigidbody rig;
    NavMeshAgent agent;
    Animator anim;
    GameObject tlGraphic;

    public float impulseMagnitude = 5;

    void Start()
    {
        if (!overrideDuration) {
            tlDuration = 5;
        }
        type = TimeLockType.Broken;
        CheckType();
        GetNecessaryComponents(type);
        if (type == TimeLockType.Prop) {
            rig = GetComponent<Rigidbody>();
            origPos = rig.position;
            origRot = rig.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == TimeLockType.Prop) {
            DebugTypeProp();
        }

    }

    private void LateUpdate() {
        if (timeLocked) {
            TimeLockDuration();
            TimeLockEffect();
        }
    }

    void DebugTypeProp() {
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
        rig.AddForce(direction * impulseMagnitude, ForceMode.Impulse);
    }

    void TimeLockEffect() {
        if (tlGraphic != null) {
            var effect = (tlTimer / tlDuration) * tlEffectMagnitude;
            var x = Random.Range(-effect, effect);
            var y = Random.Range(-effect, effect);
            var z = Random.Range(-effect, effect);
            tlGraphic.transform.localPosition = new Vector3(x, y, z);
        } else print("No graphic was found for TimeLock effect");
    }

    void TimeLockDuration() {
        tlTimer += Time.deltaTime;
        if (tlTimer >= tlDuration) {
            tlTimer = 0;
            timeLocked = false;
            ReleaseTimeLock();
        }
    }

    public void TimeLock() {
        if (type == TimeLockType.Broken) {
            print("Object's TimeLockType is Broken");
        } else {
            timeLocked = true;
            if (type == TimeLockType.Prop) {
                lockedDirection = rig.velocity;
                lockedRotation = rig.angularVelocity;
                print("Prop locked - Locked velocity vector: " + lockedDirection.x + ", " + lockedDirection.y + ", " + lockedDirection.z);
                rig.velocity = Vector3.zero;
                rig.angularVelocity = Vector3.zero;
                wasKinematic = rig.isKinematic;
                rig.isKinematic = true;
                hadGravity = rig.useGravity;
                rig.useGravity = false;
            }
            if (type == TimeLockType.Animator || hasAnimator) {
                lockedAnimSpeed = anim.speed;
                anim.speed = 0;
                if (tlGraphic.GetComponent<Animator>() != null) {
                    tlGraphic.GetComponent<Animator>().speed = 0;
                }
            }
            if (type == TimeLockType.Agent) {
                wasStopped = agent.isStopped;
                agent.isStopped = true;
            }
        }
        if (tlGraphic != null) {
            tlGraphic.SetActive(true);
        }
    }

    public void ReleaseTimeLock() {
        if (type == TimeLockType.Prop) {
            print("Released prop");
            rig.velocity = lockedDirection;
            rig.angularVelocity = lockedRotation;
            rig.isKinematic = wasKinematic;
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

    void ResetObject() {
        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;
        rig.position = origPos;
        rig.rotation = origRot;
    }

    void GetNecessaryComponents(TimeLockType tlt) {
        if (tlt == TimeLockType.Prop) {
            rig = GetComponent<Rigidbody>();
            if (rig.mass > 1) {
                tlDuration -= rig.mass;
            }
        }
        if (tlt == TimeLockType.Agent) {
            agent = GetComponent<NavMeshAgent>();
        }
        if (tlt == TimeLockType.Animator || hasAnimator) {
            if (GetComponent<Animator>() != null) {
                anim = GetComponent<Animator>();
            } else anim = GetComponentInChildren<Animator>();
        }
        if (transform.Find("TimeLockGraphic") != null) {
            tlGraphic = transform.Find("TimeLockGraphic").gameObject;
            tlGraphic.SetActive(false);
        } else Debug.LogError("No TimeLock effect graphic found on object " + gameObject.name + "!");
    }

    void CheckType() {
        if (gameObject.layer == LayerMask.NameToLayer("Props")) {
            if (GetComponent<Rigidbody>() != null) {
                type = TimeLockType.Prop;
            } else type = TimeLockType.Broken;
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
}
