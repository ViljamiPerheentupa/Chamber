using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {
    public Transform source;
    public float moveSpeed = 2.0f;

    void Update() {
        transform.position += transform.up * moveSpeed * Time.deltaTime;
    }
}
