using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShock : GunAmmoBase {
    public float forceAmount = 30.0f;
    public override void Fire(Vector3 startPos, Vector3 forward) {
        Debug.Log("Shock!");
        
        RaycastHit hit;
        if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)){
            Debug.Log("Found!");
            if (hit.rigidbody) {
                Debug.Log("Rigid!");
                Vector3 force = forward * forceAmount;
                hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
            }
        }
    }
}
