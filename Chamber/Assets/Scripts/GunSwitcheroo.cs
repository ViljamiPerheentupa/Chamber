using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunSwitcheroo", menuName = "Chamber/Player/Gun/Swicheroo", order = 0)]
public class GunSwitcheroo : GunAmmoBase
{
    public Vector3 offset;
    public LayerMask layerMask;
    public Transform missParticle;

    Vector3 oldPos;
    Vector3 newPos;
    public override void FirePress(Vector3 startPos, Vector3 forward) {
        RaycastHit hit;
        if (Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, layerMask)) {
            if (hit.collider.GetComponent<IProp>() != null) {
                newPos = hit.transform.position + offset;
                oldPos = GameObject.Find("Player").transform.position + offset;
                hit.transform.position = oldPos;
                GameObject.Find("Player").transform.position = newPos;
            } else {
                // Hit invalid object
                Instantiate(missParticle, hit.point, Quaternion.LookRotation(hit.normal));
            };

            gun.FireLineRenderer(hit.point, 2);
        } else { // Hit nothing
            gun.FireLineRenderer(startPos + forward * 100.0f, 1);
        }
        // else print("Missed Timehit");

        gun.PlayFireAnimation();
        gun.SetCurrentChamber(Gun.AmmoType.Empty);
        gun.SwapToNextChamber();
        gun.WaitForNextShot();
    }
}
