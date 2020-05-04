using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTime : GunAmmoBase {
    public override void Fire(Vector3 startPos, Vector3 forward) {
        Debug.Log("Time!");
    }
}
