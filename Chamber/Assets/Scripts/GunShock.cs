using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GunShock : GunAmmoBase {
    public Transform shockParticle;
    public Vector3 decalSize;
    public Material decalMaterial;
    public float bulletDamage = 40.0f;
    public float forceAmount = 30.0f;

    public override void FirePress(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitGeneric", hit.point);
        
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
                gunContainer.CreateDecal(hit.point, Quaternion.LookRotation(hit.normal), decalSize, decalMaterial, hit.collider.transform);
            }

            BaseHealth health = hit.collider.GetComponent<BaseHealth>();
            if (health) {
                health.TakeDamage(bulletDamage, transform);

            }

            Instantiate(shockParticle, hit.point, Quaternion.LookRotation(hit.normal));
            gunContainer.FireLineRenderer(hit.point, 0);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Gunshot", startPos);
        gunContainer.PlayFireAnimation();
        gunContainer.SetCurrentChamber(GunContainer.AmmoType.Empty);
        gunContainer.SwapToNextChamber();
        gunContainer.WaitForNextShot();
    }
}
