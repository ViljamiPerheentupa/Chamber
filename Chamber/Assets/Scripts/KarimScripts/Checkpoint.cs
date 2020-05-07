using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    Transform player;

    void Start() {
        player = GameObject.Find("Player").transform;
    }

    public void SetCheckpoint() {
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.SetCheckpoint(this);
        }
    }

    public void StartReset() {
        player.position = transform.position;
        player.eulerAngles = transform.eulerAngles;

        MouseLook ml = FindObjectOfType<MouseLook>();
        if (ml) {
            ml.StartReset(transform.eulerAngles.y);
        }

        GunContainer gc = FindObjectOfType<GunContainer>();
        if (gc) {
            gc.StartReset();
        }

        PlayerMoverNew pm = FindObjectOfType<PlayerMoverNew>();
        if (pm) {
            pm.StartReset();
        }

        PlayerLifting pl = FindObjectOfType<PlayerLifting>();
        if (pl) {
            pl.StartReset();
        }

        GunMagnet gm = FindObjectOfType<GunMagnet>();
        if (gm) {
            gm.StartReset();
        }
    }
}
