using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlider : MonoBehaviour {
    public bool startOpen;
    public Vector3 moveAmount;
    public float timeToMove;

    private Vector3 startPosition;
    private float startMoveTime;
    private bool isClosed = false;

    private void Start() {
        startPosition = transform.position;
        isClosed = !startOpen;
    }
    
    public void Open() {
        float t = timeToMove - (Time.time - startMoveTime);
        t = Mathf.Clamp(t, 0.0f, timeToMove);

        if (isClosed) {
            startMoveTime = Time.time - t;
            isClosed = false;
        }
    }

    public void Close() {
        float t = timeToMove - (Time.time - startMoveTime);
        t = Mathf.Clamp(t, 0.0f, timeToMove);

        if (!isClosed) {
            startMoveTime = Time.time - t;
            isClosed = true;
        }
    }

    private void Update() {
        float t = (Time.time - startMoveTime) / timeToMove;
        if (isClosed) {
            t = 1 - t;
        }
        t = Mathf.Clamp(t, 0, 1);
        transform.position = Vector3.Lerp(startPosition, startPosition + moveAmount, t);
    }
}
