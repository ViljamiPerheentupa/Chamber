using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootTrigger : BaseResetable {
    public UnityEvent onShockTrigger;
    public UnityEvent onMagnetTrigger;
    public UnityEvent onTimeTrigger;
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

    public void OnShockTrigger() {
        onShockTrigger.Invoke();
    }

    public void OnMagnetTrigger() {
        onMagnetTrigger.Invoke();
    }

    public void OnTimeTrigger() {
        onTimeTrigger.Invoke();
    }
}
