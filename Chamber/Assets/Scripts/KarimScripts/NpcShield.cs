using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcShield : BaseResetable {
    enum Status {
        Idle,
        Alert,
        Attack
    };

    #region Public Variables
        public Transform shieldPivot;
        public float shieldUpAngle = 45f;
        public float attackDistance = 2f;
        public float attackDamage = 60f;
        public float moveSpeed = 8f;
        public float turnSpeed = 20f;
        public float forcePush = 20.0f;
        public float pushDelay = 1f;
        public float playerDetectRange = 10.0f;
        public float detectConeAngle = 45.0f;
        public LayerMask visibilityLayerMask;
    #endregion

    #region Private Variables
        Transform target;
        float startMoveTime;
        bool isShieldUp;
        Status status;
        float nextPush;
        NavMeshAgent navigationAgent;
        bool isDead = false;
    #endregion

    void Start() {
        status = Status.Idle;
        navigationAgent = GetComponent<NavMeshAgent>();
    }

    public override void StartReset() {
        shieldPivot.gameObject.SetActive(true);
        isDead = false;
    }

    void StartMovingShield(bool up) {
        startMoveTime = Time.time;
        isShieldUp = up;
    }

    void SetShieldAngles() {
        // Calculate Shield Position
        Vector3 shieldAngles = shieldPivot.localEulerAngles;
        float t = (Time.time - startMoveTime) / 0.5f;
        if (isShieldUp) t = 1 - t;
        shieldAngles.x = Mathf.Lerp(-90f, 0f, t);
        shieldPivot.localEulerAngles = shieldAngles;
    }

    public void Die() {
        isDead = true;
        status = Status.Idle;
        shieldPivot.gameObject.SetActive(false);
    }

    void Update() {
        if (isDead) return;

        if (!target) {
            GameObject testTarget = GameObject.Find("Player");
            if (testTarget) {
                target = testTarget.transform;
            }
            else {
                return;
            }
        }
        
        SetShieldAngles();

        Vector3 diff = target.position - transform.position;

        switch(status) {
            case Status.Idle:
                if (diff.magnitude < playerDetectRange) {
                    Vector3 normdir = Vector3.Normalize(diff);
                    // TODO: Move the detect cone angle math to a private variable at start, when we're close to shipping
                    if (Vector3.Dot(normdir, transform.forward) > Mathf.Cos(Mathf.Deg2Rad * detectConeAngle)) {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position + transform.forward * 1.5f, normdir, out hit, Mathf.Infinity, visibilityLayerMask)) {
                            if (hit.collider.transform == target) {
                                status = Status.Attack;
                            }
                        }
                    }
                }
                break;
            case Status.Alert:
                break;
            case Status.Attack:
                float diffDot = Vector3.Dot(Vector3.Normalize(diff), Vector3.up);
                
                if (diffDot > Mathf.Cos(Mathf.Deg2Rad * shieldUpAngle)) {
                    if (!isShieldUp) StartMovingShield(true);
                }
                else {
                    if (isShieldUp) StartMovingShield(false);

                    if (diff.magnitude < attackDistance) {
                        // Attack
                        if (Time.time > nextPush) {
                            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                            playerHealth.TakeDamage(attackDamage, diff);
                            target.GetComponent<Rigidbody>().AddForce(transform.forward * forcePush, ForceMode.Impulse);
                            nextPush = Time.time + pushDelay;
                        }
                    }
                    else {
                        // Move Towards
                        navigationAgent.destination = target.position;
                    }
                }
                break;
       }
    }
}
