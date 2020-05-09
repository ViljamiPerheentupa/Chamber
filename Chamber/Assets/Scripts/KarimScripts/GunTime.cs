using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTime : GunAmmoBase {

    public LayerMask layerMask;
    public override void OnFire(Vector3 startPos, Vector3 forward) {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", startPos);

        RaycastHit hit;
        if (Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, layerMask)) {
            if (hit.collider.GetComponent<IProp>() != null) {
                hit.collider.GetComponent<IProp>().TimeLock();
            } else if (hit.collider.GetComponentInParent<IProp>() != null) {
                hit.collider.GetComponentInParent<IProp>().TimeLock();
            } else print("Hit object cannot be timelocked");
        }

        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }
}
