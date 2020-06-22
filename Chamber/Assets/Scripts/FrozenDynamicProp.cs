using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenDynamicProp : MonoBehaviour, IProp
{
    bool hasAnimator;
    private bool timeLocked = false;
    [Tooltip("If the GameObject has a unique TimeLock duration, check this.")]
    public bool overrideDuration = false;
    [Tooltip("The unique duration. Won't do anything if the above box isn't checked.")]
    public float tlDuration = 5;
    [Tooltip("The magnitude of the 'glitch' effect during the TimeLock.")]
    public float tlEffectMagnitude = 0.25f;

    float tlTimer = 0;
    Vector3 lockedDirection;
    Vector3 lockedRotation;
    float lockedAnimSpeed;
    bool hadGravity;
    bool wasStopped;
    RigidbodyConstraints rigidbodyConstraints;

    Rigidbody rig;

    void Start()
    {
        if (!overrideDuration) {    //If there is no override, use the default value
            tlDuration = 5;
        }
        GetNecessaryComponents();
    }

    public void TimeLock() {        //Applying the TimeLock effect
        if (!timeLocked) {      //Making sure the TimeLock isn't applied on top of an earlier TimeLock
            timeLocked = true;
           //  rig.useGravity = false;
            rig.freezeRotation = true;
            rigidbodyConstraints = rig.constraints;
            rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        } else {
            CancelInvoke();
        }

        Invoke("ReleaseTimeLock", tlDuration);
    }

    public void ReleaseTimeLock() {         //Ending the TimeLock
        CancelInvoke();
        timeLocked = false;
        // rig.useGravity = true;
        rig.freezeRotation = false;
        rig.constraints = rigidbodyConstraints;
    }

    void GetNecessaryComponents() {
        rig = GetComponent<Rigidbody>();
        if (rig.mass > 1 && !overrideDuration) {
            tlDuration -= rig.mass;     //Calculating the correct TimeLock duration, if no overwrite duration was given
        }
    }

    public void PropForce(Vector3 force, ForceMode forceMode) {     //The custom interface to add velocity to props, so it can work properly during a TimeLock
        /*if (timeLocked) {
            lockedDirection += force;
        } else*/rig.AddForce(force, forceMode);
    }

    public void PropExplosiveForce(Vector3 location, float force, float radius) {       //The same, but with the AddExplosionForce function
        if (timeLocked) {
            // TODO: Calculate "explosion" force and add it to lockedDirection
        } else rig.AddExplosionForce(force, location, radius);
    }
}
