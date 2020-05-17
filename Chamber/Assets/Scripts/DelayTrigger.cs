using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayTrigger : BaseResetable {
    public UnityEvent OnTime;
    public UnityEvent OnReset;
    public bool oneAtATime;
    public float time;

    private float releaseTime;
    private bool isTriggered;

    void Start() {
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    public void StartReset() {
        OnReset.Invoke();
    }

    void Release() {
        OnTime.Invoke();
    }

    void Update() {
        if (oneAtATime && isTriggered && Time.time > releaseTime) {
            OnTime.Invoke();
            isTriggered = false;
        }
    }

    public void TriggerDefaultTime() {
        Trigger(time);
    }

    public void Trigger(float _time) {
        releaseTime = Time.time + _time;
        isTriggered = true;

        if (!oneAtATime) {
            Invoke("Release", _time);
        }
    }
}
