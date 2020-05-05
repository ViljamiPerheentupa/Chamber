using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShockTrigger : MonoBehaviour {
    public UnityEvent onTrigger;

    public void OnTrigger() {
        onTrigger.Invoke();
    }
}
