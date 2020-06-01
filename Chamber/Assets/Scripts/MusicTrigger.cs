using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public MusicManager mm;
    public LayerMask contactLayerMask = (1 << 9);
    public string parameter;
    public int value;
    private void Start() {
        mm = GameObject.Find("MusicManager").GetComponent<MusicManager>();
    }
    private void OnTriggerEnter(Collider other) {
        if (contactLayerMask == (contactLayerMask | (1 << other.gameObject.layer))) {
            mm.SetParameter(parameter, value);
        }
    }

    public void Invoke() {
        mm.SetParameter(parameter, value);
    }
}
