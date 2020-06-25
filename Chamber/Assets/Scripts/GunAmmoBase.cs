using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAmmoBase : ScriptableObject {
    public Color color;
    protected Gun gun;
    protected Transform transform;
    protected Rigidbody rigidbody;

    public void SetGun(Gun g) {
        gun = g;
        transform = g.transform;
        rigidbody = g.GetComponent<Rigidbody>();
    }

    // To be implemented by the ammo types
    public virtual void FirePress(Vector3 startPos, Vector3 forward) {}
    public virtual void FireHold(Vector3 startPos, Vector3 forward) {}
    public virtual void FireRelease(Vector3 startPos, Vector3 forward) {}
}
