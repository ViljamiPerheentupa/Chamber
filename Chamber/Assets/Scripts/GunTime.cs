using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTime : GunAmmoBase {
    public override void OnFire(Vector3 startPos, Vector3 forward) {
        Debug.Log("Time!");

        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }
}
