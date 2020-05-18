using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagnet : GunAmmoBase {
    //[FMODUnity.EventRef]
    //[FMODUnity.EventRef]
    public string magnetizeEventPath;
    public string grappleEventPath;
    public float moveStrength = 30.0f;
    public float movePlayerStrength = 30.0f;
    public Color holdColor;
    public LayerMask magnetableLayers;
    public LayerMask magnetSurfaceLayers;
    private Rigidbody magnetTarget;
    private Vector3 targetLocation;
    [HideInInspector]
    public bool isPulling = false;
    private bool isMovingMagnetTarget = false;
    public Transform missParticle;

    private FMOD.Studio.EventInstance magnetizeEvent;
    private FMOD.Studio.EventInstance grappleEvent;

    public void StartReset() {
        magnetTarget = null;
    }

    void Update() {
        if (isPulling) {
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, movePlayerStrength * Time.deltaTime);
        }
        else if (magnetTarget && isMovingMagnetTarget) {
            magnetTarget.AddForce(moveStrength * Vector3.Normalize(targetLocation - magnetTarget.transform.position), ForceMode.Force);
        }
    }

    public override void FirePress(Vector3 startPos, Vector3 forward) {
        
        if (magnetTarget) {        
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, magnetSurfaceLayers)) {
                isMovingMagnetTarget = true;
                targetLocation = hit.point;
                gunContainer.FireLineRenderer(hit.point, 1);
                magnetTarget.AddForce(moveStrength * Vector3.Normalize(hit.point - magnetTarget.transform.position), ForceMode.Force);
                gunContainer.SetHoldMode(true);
                magnetizeEvent = FMODUnity.RuntimeManager.CreateInstance(magnetizeEventPath);
                magnetizeEvent.setParameterByName("LockOn", 1.0f);
                magnetizeEvent.start();
            }
            else {
                gunContainer.FireLineRenderer(startPos + forward * 100.0f, 1);
                gunContainer.WaitForNextShot();
                gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
                gunContainer.SetHoldMode(false);
                gunContainer.SwapToNextChamber();
            }
            gunContainer.PlayFireAnimation();
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
                else if (hit.collider.GetComponent<ShootTrigger>()) {
                    hit.collider.GetComponent<ShootTrigger>().OnMagnetTrigger();
                }
                else if (hit.rigidbody) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);
                    magnetTarget = hit.rigidbody;
                    gunContainer.PlayFireAnimation();
                    gunContainer.WaitForNextShot();
                    gunContainer.SetCurrentChamberColor(holdColor);
                }
                else {
                    Instantiate(missParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    gunContainer.PlayFireAnimation();
                    gunContainer.WaitForNextShot();
                    gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
                    gunContainer.SetHoldMode(false);
                    gunContainer.SwapToNextChamber();
                }
                
                gunContainer.FireLineRenderer(hit.point, 1);
            }
            else {
                gunContainer.FireLineRenderer(startPos + forward * 100.0f, 1);
                gunContainer.PlayFireAnimation();
                gunContainer.WaitForNextShot();
                gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
                gunContainer.SetHoldMode(false);
                gunContainer.SwapToNextChamber();
            }
        }
    }

    public override void FireRelease(Vector3 startPos, Vector3 forward) {
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
            isMovingMagnetTarget = false;
            magnetTarget = null;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
    }
}
