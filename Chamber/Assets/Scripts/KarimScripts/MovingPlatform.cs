using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    public float moveSpeed = 2.0f;
    public Transform targetPoint;
    public bool isEnabled;
    public bool automaticMove;

    public bool isOrderedToMove = false;
    public bool isMovingToEnd = true;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 targetPosition;

    void Start() {
        startPosition = transform.position;
        endPosition = targetPoint.position;
        targetPosition = endPosition;
    }

    public void ToggleMove() {
        isOrderedToMove = true;
        isMovingToEnd = !isMovingToEnd;
        targetPosition = isMovingToEnd ? endPosition : startPosition;
    }

    public void MoveTo(bool moveToEnd) {
        isOrderedToMove = true;
        isMovingToEnd = moveToEnd;
        targetPosition = isMovingToEnd ? endPosition : startPosition;
    }

    void FixedUpdate() {
        if (automaticMove || isOrderedToMove) {
            if ((transform.position - targetPosition).magnitude < 0.01f) {
                if (automaticMove) {
                    isMovingToEnd = !isMovingToEnd;
                    targetPosition = isMovingToEnd ? endPosition : startPosition;
                }
                else {
                    isOrderedToMove = false;
                }
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody) {
            other.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody) {
            other.transform.parent = null;
        }
    }
}
