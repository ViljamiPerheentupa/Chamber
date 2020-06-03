using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour {
    public LayerMask contactLayerMask = (1 << 9);
    public UnityEvent onEnter;
    public UnityEvent onExit;
    
    [Tooltip("If this is enabled, OnExit will only be called when the last item leaves the trigger, and OnEnter will only be called when the first item enters it.")]
    public bool combineTriggers = true;

    private int numItems = 0;

    private void OnTriggerEnter(Collider other) {
        if (contactLayerMask == (contactLayerMask | (1 << other.gameObject.layer))) {
            if (numItems++ == 0 || !combineTriggers) {
                onEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (contactLayerMask == (contactLayerMask | (1 << other.gameObject.layer))) {
            if (--numItems == 0 || !combineTriggers) {
                onExit.Invoke();
            }
        }
    }
}
