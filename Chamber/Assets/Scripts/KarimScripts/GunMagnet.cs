using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagnet : GunAmmoBase {
    public float moveStrength = 30.0f;
    public float movePlayerStrength = 30.0f;
    public Color holdColor;
    public LayerMask magnetableLayers;
    private Rigidbody magnetTarget;
    private Vector3 targetLocation;
    [HideInInspector]
    public bool isPulling = false;

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
            }
        }
        else {
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, magnetableLayers)) {
                if (hit.collider.gameObject.tag == "GrappleSpot") {
                    isPulling = true;
                    GetComponent<Rigidbody>().useGravity = false;
                    
                    targetLocation = hit.collider.transform.position + hit.collider.transform.forward * 1.5f;
                    gunContainer.PlayFireAnimation();
                    gunContainer.WaitForNextShot();
                    gunContainer.SetHoldMode(true);
                    gunContainer.SetCurrentChamberColor(holdColor);
                }
                else if (hit.rigidbody) {
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
            GetComponent<Rigidbody>().useGravity = true;
            isPulling = false;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
        else if (magnetTarget) {
            magnetTarget = null;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
    }
}
