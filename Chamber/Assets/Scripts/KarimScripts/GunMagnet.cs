using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagnet : GunAmmoBase {
    [FMODUnity.EventRef]
    public string magnetizeEventPath;
    [FMODUnity.EventRef]
    public string grappleEventPath;
    public float moveStrength = 30.0f;
    public float movePlayerStrength = 30.0f;
    public Color holdColor;
    public LayerMask magnetableLayers;
    private Rigidbody magnetTarget;
    private Vector3 targetLocation;
    [HideInInspector]
    public bool isPulling = false;

    private FMOD.Studio.EventInstance magnetizeEvent;
    private FMOD.Studio.EventInstance grappleEvent;

    public void StartReset() {
        magnetTarget = null;
    }

    public override void OnFire(Vector3 startPos, Vector3 forward) {
        
        if (magnetTarget) {        
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
                targetLocation = hit.point;
                magnetTarget.AddForce(moveStrength * Vector3.Normalize(hit.point - magnetTarget.transform.position), ForceMode.Force);
                gunContainer.SetHoldMode(true);
                gunContainer.PlayFireAnimation();
                magnetizeEvent = FMODUnity.RuntimeManager.CreateInstance(magnetizeEventPath);
                magnetizeEvent.setParameterByName("LockOn", 1.0f);
                magnetizeEvent.start();
            }
        }
        else {
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, magnetableLayers)) {
                if (hit.collider.gameObject.tag == "GrappleSpot") {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);
                    grappleEvent = FMODUnity.RuntimeManager.CreateInstance(grappleEventPath);
                    grappleEvent.setParameterByName("LockOn", 1.0f);
                    grappleEvent.start();
                    isPulling = true;
                    GetComponent<Rigidbody>().useGravity = false;
                    
                    targetLocation = hit.collider.transform.position + hit.collider.transform.forward * 1.5f;
                    gunContainer.PlayFireAnimation();
                    gunContainer.WaitForNextShot();
                    gunContainer.SetHoldMode(true);
                    gunContainer.SetCurrentChamberColor(holdColor);
                }
                else if (hit.rigidbody) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);
                    magnetTarget = hit.rigidbody;
                    gunContainer.PlayFireAnimation();
                    gunContainer.WaitForNextShot();
                    gunContainer.SetCurrentChamberColor(holdColor);
                }
            }
            else {
                gunContainer.WaitForNextShot();
            }
        }
    }
    
    public override void OnFireHold(Vector3 startPos, Vector3 forward) {
        if (isPulling) {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, movePlayerStrength * Time.deltaTime);
        }
        else if (magnetTarget) {
            magnetTarget.AddForce(moveStrength * Vector3.Normalize(targetLocation - magnetTarget.transform.position), ForceMode.Force);
        }
    }

    public override void OnFireRelease(Vector3 startPos, Vector3 forward) {
        if (isPulling) {
            grappleEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            grappleEvent.release();
            GetComponent<Rigidbody>().useGravity = true;
            isPulling = false;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
        else if (magnetTarget) {
            magnetizeEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            magnetizeEvent.release();
            magnetTarget = null;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
    }
}
