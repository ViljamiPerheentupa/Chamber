using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTime : GunAmmoBase {
    public Transform missParticle;
    public LayerMask layerMask;
    public override void FirePress(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if (Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, layerMask)) {
            
            if (hit.collider.GetComponent<IProp>() != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);
                hit.collider.GetComponent<IProp>().TimeLock();
            } else if (hit.collider.GetComponentInParent<IProp>() != null) {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);
                hit.collider.GetComponentInParent<IProp>().TimeLock();
            } else {
                // Hit invalid object
                Instantiate(missParticle, hit.point, Quaternion.LookRotation(hit.normal));
            };
            
            gunContainer.FireLineRenderer(hit.point, 2);
        }
        else { // Hit nothing
            gunContainer.FireLineRenderer(startPos + forward * 100.0f, 1);
        }
        // else print("Missed Timehit");

        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }
}
