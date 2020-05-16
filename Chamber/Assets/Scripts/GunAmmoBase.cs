using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAmmoBase : MonoBehaviour {
    public Color color;
    protected GunContainer gunContainer;

    void Start() {
        gunContainer = GetComponent<GunContainer>();
    }

    // To be implemented by the ammo types
    public virtual void FirePress(Vector3 startPos, Vector3 forward) {}
    public virtual void FireHold(Vector3 startPos, Vector3 forward) {}
    public virtual void FireRelease(Vector3 startPos, Vector3 forward) {}
}
