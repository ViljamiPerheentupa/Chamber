using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GunShock : GunAmmoBase {
    public GameObject decal;
    public float forceAmount = 30.0f;

    public override void OnFire(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
            HitWall(hit.point, forward, hit.normal);

            ShockTrigger st = hit.collider.GetComponent<ShockTrigger>();
            if (st) {
                st.OnTrigger();
            }

            if (hit.rigidbody) {
                Vector3 force = forward * forceAmount;
                hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
            }
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Gunshot", startPos);
        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }

    void HitWall(Vector3 pos, Vector3 forward, Vector3 normal) {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", pos);
        Vector3 decpos = pos - (forward * 0.001f);
        gunContainer.CreateDecal(decal, decpos, normal);
    }
}
