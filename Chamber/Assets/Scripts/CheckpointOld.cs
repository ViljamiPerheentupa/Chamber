using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointOld : MonoBehaviour {

    public RestoreGroup restoreGroup;
    CheckpointManagerOld cm;

    private void Start() {
        cm = FindObjectOfType<CheckpointManagerOld>();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            if(cm != null)
                cm.SetActiveCheckpoint(this);
            else
                print("No CheckpointManager found in Scene");
        }
    }
    public void RestoreGroup() {
        if(restoreGroup != null)
            restoreGroup.RestoreObjects();
        else
            print("No RestoreGroup assigned");
    }
}
