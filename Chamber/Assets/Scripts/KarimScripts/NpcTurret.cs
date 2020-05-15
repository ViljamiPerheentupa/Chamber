using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTurret : BaseResetable, IProp {
    public Vector3 decalSize;
    public Material decalMaterial;
    public float bulletForce = 20.0f;
    public float playerDetectRange = 10.0f;
    public float playerLoseRange = 12.0f;
    public float detectConeAngle = 45.0f;
    // public float playerDetectTime = 1.0f;
    public float playerForgetTime = 4.0f;
    public float bulletHitRange = Mathf.Infinity;
    public float minimumBulletDamage = 10.0f;
    public float maximumBulletDamage = 50.0f;
    public float minimumBulletDelay = 1.0f;
    public float maximumBulletDelay = 0.2f;
    public float minimumBulletSpread = 30.0f;
    public float maximumBulletSpread = 5.0f;
    public float timeToMaximum = 2.0f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
    public float trackingSpeed = 0.3f;
    public float minBarrelPitch = -60.0f;
    public float maxBarrelPitch = 60.0f;
    public Transform turretBase;
    public Transform barrelPivot;
    public Transform firePoint;
    public float turningIdleSpeed = 20.0f;
    public bool shouldPingPong = true;
    public float pingPongAngle = 45.0f;
    public LayerMask layerMask;
    public bool isEnabled = true;

    // Private
    private float lastSeePlayer = 0.0f;
    private float timeStartedShooting = 0.0f;
    private float nextShot = 0.0f;
    private Vector3 targetDirection;
    private Vector3 targetDirectionVelocity;
    private bool foundPlayer = false;
    private DecalManager decalManager;
    private LineRenderer lineRenderer;
    private bool isSpinningCW;
    public Transform shell;
    public float timeLockDuration = 20.0f;
    public float timeLockShootSpeed = 0.8f;
    public float timeLockShootDeviation = 10.0f;

    // Reset data
    private bool isEnabledDefault;
    private Quaternion defaultRotation;
    private Quaternion defaultBaseRotation;
    private bool isDead;
    private bool isTimeLocked = false;
    private float releaseTimeLockTime;

    void Start() {
        decalManager = GameObject.Find("Decals").GetComponent<DecalManager>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = isEnabled;

        isEnabledDefault = isEnabled;
        defaultRotation = transform.rotation;
        defaultBaseRotation = turretBase.rotation;
    
        CheckpointManager cm = FindObjectOfType<CheckpointManager>();
        if (cm) {
            cm.RegisterResetable(this);
        }
    }

    // Update is called once per frame
    public override void StartReset() {
        SetState(isEnabledDefault);
        transform.rotation = defaultRotation;
        turretBase.rotation = defaultBaseRotation;
        barrelPivot.rotation = new Quaternion();
        isDead = false;
        isTimeLocked = false;
        releaseTimeLockTime = 0.0f;
    }

    public void SetState(bool state) {
        isEnabled = state;
        lineRenderer.enabled = state;
        
        if (!state) {
            foundPlayer = false;
        }
    }

    public void SetDead(bool state) {
        isDead = state;
    }
    public void TimeLock() {        //Applying the TimeLock effect
        if (!isTimeLocked) {      //Making sure the TimeLock isn't applied on top of an earlier TimeLock
            isTimeLocked = true;
            
            foundPlayer = true;
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

    void Update() {
        lineRenderer.enabled = isEnabled && !isDead;
        
        if (isEnabled && !isDead) {
            if (lineRenderer) {
                lineRenderer.SetPosition(0, firePoint.position);
                
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.up, out hit, bulletHitRange, layerMask)) {
                    lineRenderer.SetPosition(1, hit.point);
                }
                else {
                    lineRenderer.SetPosition(1, firePoint.position + firePoint.up * bulletHitRange);
                }
            }

            Transform target = GameObject.Find("Player").transform;
            Vector3 directDirection = (target.position - firePoint.position);
            float distanceToPlayer = directDirection.magnitude;

            // TimeLock Enabled
            if (isTimeLocked) {
                if (Time.time > nextShot) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EShoot", transform.position);
                    
                    // Calculate spread
                    float deviation = Random.Range(0.0f, timeLockShootDeviation);
                    float angle = Random.Range(0.0f, 360.0f);
                    Vector3 dir = Vector3.forward;
                    dir = Quaternion.AngleAxis(deviation, Vector3.up) * dir;
                    dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    dir = Quaternion.LookRotation(-firePoint.up) * dir;

                    Transform sh = Instantiate(shell, firePoint.position, Quaternion.LookRotation(dir));
                    sh.GetComponent<BulletProjectile>().source = transform;
                    
                    nextShot = Time.time + timeLockShootSpeed;
                }

                if (Time.time > releaseTimeLockTime) {
                    ReleaseTimeLock();
                }
            }
            else if (foundPlayer) {
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if (playerHealth) {
                    if (playerHealth.isDead) {
                        return;
                    }
                }
                
                if (distanceToPlayer > playerLoseRange) {
                    foundPlayer = false;
                    Vector3 fixAngles = barrelPivot.localEulerAngles;
                    fixAngles.x = 0;
                    barrelPivot.localEulerAngles = fixAngles;
                    return;
                }

                Vector3 newTarget = Vector3.Normalize(target.position - transform.position);
                targetDirection = Vector3.SmoothDamp(targetDirection, newTarget, ref targetDirectionVelocity, trackingSpeed);
                Quaternion q = Quaternion.LookRotation(targetDirection, Vector3.up);
                Vector3 baseAngles = turretBase.localEulerAngles;
                baseAngles.y = q.eulerAngles.y;
                turretBase.localEulerAngles = baseAngles;
                Vector3 barrelAngles = barrelPivot.localEulerAngles;
                barrelAngles.x = Mathf.Clamp(q.eulerAngles.x, minBarrelPitch, maxBarrelPitch);
                barrelPivot.localEulerAngles = barrelAngles;

                if (Time.time > nextShot) {
                    // Detect if player is still wit
                    float t = (Time.time - timeStartedShooting) / timeToMaximum;
                    t = animationCurve.Evaluate(t);

                    nextShot = Time.time + Mathf.Lerp(minimumBulletDelay, maximumBulletDelay, t);
                    float bulletSpread = Mathf.Lerp(minimumBulletSpread, maximumBulletSpread, t);

                    // Calculate spread
                    float deviation = Random.Range(0.0f, bulletSpread);
                    float angle = Random.Range(0.0f, 360.0f);
                    Vector3 dir = Vector3.forward;
                    dir = Quaternion.AngleAxis(deviation, Vector3.up) * dir;
                    dir = Quaternion.AngleAxis(angle, Vector3.forward) * dir;
                    dir = Quaternion.LookRotation(targetDirection) * dir;

                    RaycastHit hit;
                    if(Physics.Raycast(firePoint.position, dir, out hit, bulletHitRange, layerMask)) {
                        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EShoot", transform.position);

                        if (hit.collider.gameObject.layer == 20) {
                            if (hit.rigidbody) {
                                Vector3 force = Vector3.Normalize(dir) * bulletForce;
                                hit.rigidbody.AddForceAtPosition(force, hit.point, ForceMode.Impulse);
                            }
                        }
                        else if (hit.collider.gameObject.layer == 12) {
                            decalManager.NewDecal(hit.point, Quaternion.LookRotation(hit.normal), decalSize, decalMaterial, hit.collider.transform);
                        }

                        BaseHealth health = hit.collider.GetComponent<BaseHealth>();
                        if (health) {
                            float bulletDamage = Mathf.Lerp(minimumBulletDamage, maximumBulletDamage, t);
                            health.TakeDamage(bulletDamage, transform);

                        }
                    }
                }
            }
            else {
                Vector3 angles = turretBase.localEulerAngles;
                if (shouldPingPong) {
                    if (isSpinningCW) {
                        angles.y += turningIdleSpeed * Time.deltaTime;
                        if (angles.y > 180.0f + pingPongAngle) {
                            isSpinningCW = false;
                        }
                    }
                    else {
                        angles.y -= turningIdleSpeed * Time.deltaTime;
                        if (angles.y < 180.0f - pingPongAngle) {
                            isSpinningCW = true;
                        }
                    }
                }
                else {
                    angles.y += turningIdleSpeed * Time.deltaTime;
                }
                turretBase.localEulerAngles = angles;


                if (distanceToPlayer < playerDetectRange) {
                    Vector3 normdir = Vector3.Normalize(directDirection);
                    // TODO: Move the detect cone angle math to a private variable at start, when we're close to shipping
                    if (Vector3.Dot(normdir, firePoint.up) > Mathf.Cos(Mathf.Deg2Rad * detectConeAngle)) {
                        RaycastHit hit;
                        if (Physics.Raycast(firePoint.position, normdir, out hit, Mathf.Infinity, layerMask)) {
                            if (hit.collider.transform == target) {
                                foundPlayer = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
