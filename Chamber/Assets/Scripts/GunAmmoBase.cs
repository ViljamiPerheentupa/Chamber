using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAmmoBase : MonoBehaviour {
    public Color color;

    // To be implemented by the ammo types
    public virtual void Fire(Vector3 startPos, Vector3 forward) {}
}
