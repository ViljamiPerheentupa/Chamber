using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMagnet : GunAmmoBase {
    public LayerMask magnetableLayers;
    private Rigidbody magnetTarget;

    public override void Fire(Vector3 startPos, Vector3 forward) {
        if (magnetTarget) {
            Debug.Log("Magnet: Moving Target!");
        
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity)) {
                magnetTarget.AddForce(4.0f * (hit.point - magnetTarget.transform.position), ForceMode.Impulse);
                magnetTarget = null;
            }
        }
        else {
            RaycastHit hit;
            if(Physics.Raycast(startPos, forward, out hit, Mathf.Infinity, magnetableLayers)) {
                if (hit.rigidbody) {
                    Debug.Log("Magnet: Found Target!");
                    magnetTarget = hit.rigidbody;
                }
            }
            else {
                Debug.Log("Magnet: Invalid Target!");
            }
        }
    }
}
