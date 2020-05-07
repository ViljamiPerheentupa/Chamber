using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagnet : GunAmmoBase {
    public float moveStrength;
    public Color holdColor;
    public LayerMask magnetableLayers;
    private Rigidbody magnetTarget;
    private Vector3 targetLocation;

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
                if (hit.rigidbody) {
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
        if (magnetTarget) {
            magnetTarget.AddForce(moveStrength * Vector3.Normalize(targetLocation - magnetTarget.transform.position), ForceMode.Force);
        }
    }

    public override void OnFireRelease(Vector3 startPos, Vector3 forward) {
        if (magnetTarget) {
            magnetTarget = null;
            gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
            gunContainer.SetHoldMode(false);
            gunContainer.SwapToNextChamber();
            gunContainer.WaitForNextShot();
        }
    }
}
