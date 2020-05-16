using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShockTrigger : BaseResetable {
    public UnityEvent onTrigger;
    public UnityEvent onReset;

    void Start() {
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public override void StartReset() {
        onReset.Invoke();
    }

    public void OnTrigger() {
        onTrigger.Invoke();
    }
}
