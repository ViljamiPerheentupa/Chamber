using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public List<GameObject> hitspots = new List<GameObject>();
    public int hitspotCount;
    void Start()
    {
        foreach (EnemyHitspot eh in GetComponentsInChildren<EnemyHitspot>()) {
            hitspots.Add(eh.gameObject);
        }
        hitspotCount = hitspots.Count;
    }

    void Update()
    {
        if (!IsAlive()) {
            GetComponent<EnemyBehaviour>().Death();
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EDeath", transform.position);
            Destroy(gameObject);
        }
    }

    bool IsAlive() {
        return hitspots.Count > 0;
    }
}
