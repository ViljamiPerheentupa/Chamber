using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicProp : MonoBehaviour, IProp
{
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
    Animator anim;
    GameObject tlGraphic;

    void Start()
    {
        if (!overrideDuration) {    //If there is no override, use the default value
            tlDuration = 5;
        }
        GetNecessaryComponents();
        origPos = rig.position;
        origRot = rig.rotation;
    }

    private void LateUpdate() {
        if (timeLocked) { //Put the actual effects into play
            TimeLockDuration();
            TimeLockEffect();
        }
    }

    void TimeLockEffect() {     //The graphical effect of the TimeLock
        if (tlGraphic != null) {
            var effect = (tlTimer / tlDuration) * tlEffectMagnitude;
            var x = Random.Range(-effect, effect);      //Randomizing the "shakiness" of the effect
            var y = Random.Range(-effect, effect);
            var z = Random.Range(-effect, effect);
            tlGraphic.transform.localPosition = new Vector3(x, y, z);
        }
    }

    void TimeLockDuration() {       //This function counts down the time of the effect
        tlTimer += Time.deltaTime;
        SlowedPhysics();        //The slowed down physics are applied while the duration is still counting down
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
            timeLocked = true;
            lockedDirection = rig.velocity;
            lockedRotation = rig.angularVelocity;
            print("Prop locked - Locked velocity vector: " + lockedDirection.x + ", " + lockedDirection.y + ", " + lockedDirection.z);
            //rig.velocity = Vector3.zero;
            //rig.angularVelocity = Vector3.zero;
            hadGravity = rig.useGravity;
            if (tlGraphic == null) {
                print("No graphic was found for TimeLock effect");
            }
            rig.useGravity = false;
        } else tlTimer = 0;     //Reset the TimeLock duration if a new one is applied before the old one ended
    }

    public void ReleaseTimeLock() {         //Ending the TimeLock
        print("Released prop");
        timeLocked = false;
        rig.velocity = lockedDirection;
        rig.angularVelocity = lockedRotation;
        rig.useGravity = hadGravity;
        if (hasAnimator) {
            anim.speed = lockedAnimSpeed;
            if (tlGraphic.GetComponent<Animator>() != null) {
                tlGraphic.GetComponent<Animator>().speed = lockedAnimSpeed;
            }
        }
        if (tlGraphic != null) {
            tlGraphic.SetActive(false);
        }
    }

    void GetNecessaryComponents() {
        rig = GetComponent<Rigidbody>();
        if (rig.mass > 1 && !overrideDuration) {
            tlDuration -= rig.mass;     //Calculating the correct TimeLock duration, if no overwrite duration was given
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
