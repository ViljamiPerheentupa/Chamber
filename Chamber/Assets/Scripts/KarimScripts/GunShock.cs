using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GunShock : GunAmmoBase {
    public GameObject decal;
    public float bulletDamage = 40.0f;
    public float forceAmount = 30.0f;

    public override void OnFire(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitElectric", hit.point);
        
            ShockTrigger st = hit.collider.GetComponent<ShockTrigger>();
            if (st) {
                st.OnTrigger();
            }

            if (hit.collider.gameObject.layer == 20) {
                if (hit.rigidbody) {
                    Vector3 force = forward * forceAmount;
                    hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
                }
            }
            else if (hit.collider.gameObject.layer == 12) {
                HitWall(hit.point, forward, hit.normal);
            }

            BaseHealth health = hit.collider.GetComponent<BaseHealth>();
            if (health) {
                health.TakeDamage(bulletDamage);

            }
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Gunshot", startPos);
        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }

    void HitWall(Vector3 pos, Vector3 forward, Vector3 normal) {
        Vector3 decpos = pos - (forward * 0.001f);
        gunContainer.CreateDecal(decal, decpos, normal);
    }
}
