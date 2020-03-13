using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    public RestoreGroup restoreGroup;
    CheckpointManager cm;

    private void Start() {
        cm = FindObjectOfType<CheckpointManager>();
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
