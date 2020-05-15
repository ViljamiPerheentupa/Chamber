using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTime : GunAmmoBase {

    public LayerMask layerMask;
    public override void FirePress(Vector3 startPos, Vector3 forward) {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);

        RaycastHit hit;
        if (Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, layerMask)) {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.GetComponent<IProp>() != null) {
                Debug.Log("I should timelock");
                hit.collider.GetComponent<IProp>().TimeLock();
            } else if (hit.collider.GetComponentInParent<IProp>() != null) {
                Debug.Log("Parent should timelock");
                hit.collider.GetComponentInParent<IProp>().TimeLock();
            } else print("Hit object cannot be timelocked");
        }
        else print("Missed Timehit");

        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }
}
