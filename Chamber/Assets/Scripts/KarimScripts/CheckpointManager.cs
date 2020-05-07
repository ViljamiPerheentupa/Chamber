using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

    List<BaseResetable> resetables = new List<BaseResetable>();
    public Checkpoint checkpoint;

    void Update() {
        if (Input.GetKeyDown("p")) {
            StartReset();
        }
    }

    public void SetCheckpoint(Checkpoint cp) {
        checkpoint = cp;
    }

    public void RegisterResetable(BaseResetable resetable) {
        resetables.Add(resetable);
    }

    void StartReset() {
        Debug.Log("Resetting");
        if (checkpoint) {
            checkpoint.StartReset();
        }
        else {
            Debug.LogError("INVALID CHECKPOINT OR NO CHECKPOINT FOUND");
        }

        /*GameObject decalManagerObj = GameObject.Find("Decals");
        if (decalManagerObj) {
            DecalManager decalManager = decalManagerObj.GetComponent<DecalManager>();
            if (decalManager) {
                decalManager.ClearAll();
            }
        }*/

        foreach (BaseResetable r in resetables) {
            r.StartReset();
        }
    }
}
