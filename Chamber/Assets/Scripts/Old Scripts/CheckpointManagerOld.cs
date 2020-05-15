using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManagerOld : MonoBehaviour {
    public CheckpointOld cp;
    public void SetActiveCheckpoint(CheckpointOld c) {
        cp = c;
    }

    public void ResetToCheckpoint() {
        // Reset ResetGroup of active Checkpoint
        if(cp != null)
            cp.RestoreGroup();
    }

    private void Update() {
        // For testing reset
        if(Input.GetKeyDown(KeyCode.R)) {
            ResetToCheckpoint();
        }
    }
}
