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
    public virtual void OnFire(Vector3 startPos, Vector3 forward) {}
    public virtual void OnFireHold(Vector3 startPos, Vector3 forward) {}
    public virtual void OnFireRelease(Vector3 startPos, Vector3 forward) {}
}
