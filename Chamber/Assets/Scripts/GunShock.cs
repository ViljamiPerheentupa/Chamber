﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "GunShock", menuName = "Chamber/Player/Gun/Shock", order = 0)]
public class GunShock : GunAmmoBase {
    public Transform shockParticle;
    public DecalData decalData;
    public float bulletDamage = 40.0f;
    public float forceAmount = 30.0f;

    public override void FirePress(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/HitGeneric", hit.point);

            ShootTrigger st = hit.collider.GetComponent<ShootTrigger>();
            if (st) {
                st.OnShockTrigger();
            }
            
            if (hit.collider.gameObject.layer == 20) {
                if (hit.rigidbody) {
                    Vector3 force = forward * forceAmount;
                    hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
                }
            }
            else if (hit.collider.gameObject.layer == 12) {
                gun.CreateDecal(hit.point, Quaternion.LookRotation(hit.normal), decalData.decalSize, decalData.decalMaterial, hit.collider.transform);
            }

            BaseHealth health = hit.collider.GetComponent<BaseHealth>();
            if (health) {
                health.TakeDamage(bulletDamage, gun.transform);

            }

            Instantiate(shockParticle, hit.point, Quaternion.LookRotation(hit.normal));
            gun.FireLineRenderer(hit.point, 0);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Gunshot", startPos);
        gun.PlayFireAnimation();
        gun.SetCurrentChamber(Gun.AmmoType.Empty);
        gun.SwapToNextChamber();
        gun.WaitForNextShot();
    }
}
