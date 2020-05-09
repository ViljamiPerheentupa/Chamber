using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetableRigidbody : BaseResetable {
    private Rigidbody rb;
    private Vector3 velocity;

    void Start() {
        rb = GetComponent<Rigidbody>();
        velocity = rb.velocity;
        
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public override void StartReset() {
        rb.velocity = velocity;
    }
}
