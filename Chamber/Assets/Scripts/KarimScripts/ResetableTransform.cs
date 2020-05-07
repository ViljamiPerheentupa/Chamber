using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetableTransform : BaseResetable {
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    // Start is called before the first frame update
    void Start() {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
        
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    // Update is called once per frame
    public override void StartReset() {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }
}
