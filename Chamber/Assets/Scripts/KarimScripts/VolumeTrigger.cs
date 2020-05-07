using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour {
    public LayerMask contactLayerMask = (1 << 9);
    public UnityEvent onEnter;
    public UnityEvent onExit;

    private void OnTriggerEnter(Collider other) {
        if (contactLayerMask == (contactLayerMask | (1 << other.gameObject.layer))) {
            onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (contactLayerMask == (contactLayerMask | (1 << other.gameObject.layer))) {
            onExit.Invoke();
        }
    }
}
