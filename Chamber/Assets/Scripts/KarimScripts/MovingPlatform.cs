﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : BaseResetable, IProp {
    #region Public Variables
    public float moveSpeed = 3.0f;
    public Transform targetPoint;
    public bool isEnabled;
    public bool automaticMove;
    public float timeLockSpeed = 0.1f;

    public float timeLockDuration = 10.0f;
    #endregion

    #region Private Variables
    private bool isTimeLocked;
    private float releaseTimeLockTime;
    private bool isDefaultEnabled;
    private bool isOrderedToMove = false;
    private bool isMovingToEnd = true;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 targetPosition;
    #endregion

    void Start() {
        startPosition = transform.position;
        endPosition = targetPoint.position;
        targetPosition = endPosition;
        isDefaultEnabled = isEnabled;
    
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    // Update is called once per frame
    public override void StartReset() {
        isEnabled = isDefaultEnabled;
        isTimeLocked = false;
        releaseTimeLockTime = 0.0f;
        targetPosition = startPosition;
        isOrderedToMove = false;
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

    public void TimeLock() {        //Applying the TimeLock effect
        if (!isTimeLocked) {      //Making sure the TimeLock isn't applied on top of an earlier TimeLock
            isTimeLocked = true;
            
            releaseTimeLockTime = Time.time + timeLockDuration;
        } else {
            releaseTimeLockTime = Time.time + timeLockDuration;
        }
    }

    public void ReleaseTimeLock() {
        isTimeLocked = false;
    }

    public void PropForce(Vector3 force, ForceMode forceMode) {

    }

    public void PropExplosiveForce(Vector3 location, float force, float radius) {

    }

    void FixedUpdate() {
        if (isTimeLocked && Time.time > releaseTimeLockTime) {
            ReleaseTimeLock();
        }
        
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
            
            float currentMoveSpeed = isTimeLocked ? timeLockSpeed : moveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);
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
