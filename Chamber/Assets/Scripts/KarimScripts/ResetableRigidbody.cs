using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetableRigidbody : BaseResetable {
    private Rigidbody rigidbody;
    private Vector3 velocity;

    void Start() {
        rigidbody = GetComponent<Rigidbody>();
        velocity = rigidbody.velocity;
        
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public override void StartReset() {
        rigidbody.velocity = velocity;
    }
}
