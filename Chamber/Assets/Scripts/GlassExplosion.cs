using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassExplosion : MonoBehaviour
{
    public Transform explosionPoint;
    public Transform soundPoint;
    public float explosionForce = 5;
    void Start()
    {
        Rigidbody[] rigs = gameObject.GetComponentsInChildren<Rigidbody>();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EDeath", soundPoint.position);
        foreach (Rigidbody rig in rigs) {
            rig.AddExplosionForce(explosionForce, explosionPoint.position, explosionForce);
            Destroy(rig.gameObject, 15);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
