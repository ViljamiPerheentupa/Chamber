using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public AnimationCurve moveCurve;
    Vector3 originalPosition;
    public Vector3 target;
    public float moveSpeed;

    void Move() {
        var t = Mathf.PingPong(Time.time * moveSpeed, 1);
        t = moveCurve.Evaluate(t);
        transform.position = Vector3.Lerp(originalPosition, target, t);
    }

    void Start() {
        originalPosition = transform.position;

        target = new Vector3(   transform.position.x + target.x,
                                transform.position.y + target.y,
                                transform.position.z + target.z);
    }


    void Update() {
        Move();
    }
}
