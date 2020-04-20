using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBehaviour : MonoBehaviour
{
    public enum Enemystate { Idling, Alerted, Hunting, Shooting, Exploding, Hiding, Sneaking, Stunned, Distracted };

    public Enemystate es;
    Enemystate memES;
    Enemystate previousES;

    float[] detectionDistances = new float[] { 25f, 35f, 35f, 35f, 35f, 35f, 35f, 0f, 0f };

    GameObject player;
    NavMeshAgent agent;
    public LayerMask layerMask;

    // Sensing distances
    float viewAngle = 90f;
    float distanceToMoveWhenAlerted = .5f;
    float proximityDistance = 1.5f;

    // Times
    float timer;
    float alertTime = 5f;
    float huntTime = 2.5f;
    public float stunTime = 1f;
    float hideTime = 10f;
    float distractionTime = 10f;

    UnityAction[] actions;

    // AI Hiding search grid
    Vector3[] hidingPointOffsets;
    int hidingSpotCircles = 4;
    int hidingSpotsPerCircle = 8;
    float hidingSpotOffsetFactor = 5f;

    // Explosive stuff
    public bool explosive;
    public bool isDumb;
    public Transform dumbTarget;
    public float explodeDistance = 2f;
    public Color changeColor;
    public float explodeTime = 1.5f;
    public LayerMask explodeMask;
    public int explodeDamage = 40;
    public float explosionForce = 500;
    public float explosionRadius = 5;
    public float playerExplosionForce = 1.5f;
    public UnityEvent onExplosion;

    // Shooting stuff
    public GameObject line1;
    public GameObject line2;
    LineRenderer laser;
    LineRenderer laser2;
    public LayerMask shootMask;
    public LayerMask playerMask;
    public LayerMask environmentMask;
    RaycastHit hit;
    float aimTimer = 0;
    public float aimTime = 2.5f;
    float shootTimer = 0;
    public float timeToShoot = 0.5f;
    float cdTimer = 0;
    public float shootCooldown = 3;
    bool shootCD = false;
    bool shooting = false;
    public int damage = 40;
    public Transform gunPos;
    public float speed = 0.5f;
    public Vector3 vectOffset;
    bool aiming = false;

    FMOD.Studio.EventInstance Laserburn;

    Material mat;
    Color originalColor;


    void Start() {
        if (!isDumb) {
            es = Enemystate.Idling;
        } else es = Enemystate.Exploding;
        if (!explosive) {
            var laser1Prefab = Instantiate(line1, Vector3.zero, Quaternion.Euler(0, 0, 0));
            laser = laser1Prefab.GetComponent<LineRenderer>();
            var laser2Prefab = Instantiate(line2, Vector3.zero, Quaternion.Euler(0, 0, 0));
            laser2 = laser2Prefab.GetComponent<LineRenderer>();
            Laserburn = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/EChargeAim");
            Laserburn.start();
        }
        player = GameObject.Find("PlayerBody");
        agent = GetComponent<NavMeshAgent>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        originalColor = mat.color;
        actions = new UnityAction[] { Idling, Alerted, Hunting, Shooting, Exploding, Hiding, Sneaking, Stunned, Distracted };
        //layerMask = LayerMask.GetMask(new string[] { "Environment" });
        hidingPointOffsets = new Vector3[hidingSpotsPerCircle * hidingSpotCircles];
        for(int i = 0; i < hidingPointOffsets.Length; i++) {
            for(int j = 0; j < hidingSpotCircles; j++) {
                for(int k = 0; k < hidingSpotsPerCircle; k++) {
                    Quaternion rotAmount = Quaternion.Euler(0, (360f / hidingSpotsPerCircle) * (1 + k), 0);
                    hidingPointOffsets[k + j * hidingSpotsPerCircle] = rotAmount * new Vector3(0, 0, hidingSpotOffsetFactor * (1 + j));
                }
            }
        }
    }

    #region Enemy states
    void Idling() {
        if (!isDumb) {
            if (previousES != Enemystate.Idling) {
                print("Idling");
            }

            if (PlayerSeen()) {
                SawPlayer();
                return;
            }

            if (PlayerHeard()) {
                HeardPlayer();
                es = Enemystate.Alerted;
            }
        }
    }

    void Alerted() {
        if (!isDumb) {
            if (previousES != Enemystate.Alerted) {
                timer = Time.time + alertTime;
                print("Alerted");
            }

            if (PlayerSeen()) {
                SawPlayer();
            }

            if (PlayerHeard()) {
                HeardPlayer();
            }

            if (Time.time > timer) {
                es = Enemystate.Idling;
            }
        }
    }

    void Hunting() {
        if (!isDumb) {
            if (!explosive) {
                aiming = false;
                print("returning to hunting");
            }
            if (previousES != Enemystate.Hunting) {
                timer = Time.time + huntTime; ;
                print("Hunting");
            }

            if (PlayerSeen() && explosive) {
                GoTo(player.transform.position);
                return;
            } else if (PlayerSeen()) {
                es = Enemystate.Shooting;
            }

            if (Time.time > timer) {
                es = Enemystate.Alerted;
            }
        }
    }

    void Shooting() {
        if(previousES != Enemystate.Shooting) {
            if (!explosive) {
                agent.destination = transform.position;
                aiming = true;
            }
            print("Shooting");
        } else if (!aiming || shootCD) {
            es = Enemystate.Hunting;
        }
    }

    void Exploding() {
        if(previousES != Enemystate.Exploding) {
            // Explode
            print("Exploding");
        } else {
            if(explosive) {
                if(isDumb)
                    GoTo(dumbTarget.position);
                else
                    GoTo(player.transform.position);
                if(Vector3.Distance(transform.position, agent.destination) < explodeDistance) {
                    GoTo(transform.position);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EChargeExp", transform.position);
                    print("Exploding soon" + Time.time);
                    Invoke("Explode", explodeTime);
                    timer = 0f;
                    explosive = false;
                }
            } else {
                ChangeColor();
            }

        }
    }

    void Hiding() {
        if(previousES != Enemystate.Hiding) {
            print("Hiding");
            timer = Time.time + hideTime;

            // Scan for possible places to hide
            for(int i = 0; i < hidingPointOffsets.Length; i++) {
                // Is not inside wall
                var p = transform.position + hidingPointOffsets[i];
                Collider[] hitColliders = Physics.OverlapSphere(p, 0.1f, layerMask);
                if(hitColliders.Length == 0) {
                    // Player does not see the point
                    var pPos = player.transform.position;
                    if(Physics.Raycast(p, pPos, Vector3.Distance(p, pPos), layerMask)) {
                        GoTo(p);
                        return;
                    }
                }
            }
        }

        if(Time.time > timer) {
            es = Enemystate.Alerted;
        }
    }

    void Sneaking() {
        if(previousES != Enemystate.Sneaking) {
            print("Sneaking");
        }
    }

    void Stunned() {
        if(previousES != Enemystate.Stunned) {
            timer = Time.time + stunTime;
            agent.destination = transform.position;
            print("Stunned");
        }
        if(Time.time > timer) {
            es = Enemystate.Alerted;
        }
    }

    void Distracted() {
        if(previousES != Enemystate.Distracted) {
            timer = Time.time + distractionTime;
            print("Distracted");
        }
        if(Time.time > timer) {
            es = Enemystate.Alerted;
        }
    }

    #endregion

    #region Actions

    void SawPlayer() {
        if (!isDumb) {
            if (explosive)
                es = Enemystate.Exploding;
            else
                es = Enemystate.Hunting;
        }
    }

    void HeardPlayer() {
        if (!isDumb) {
            var pos = transform.position;
            GoTo((Vector3.Normalize(player.transform.position - pos) * distanceToMoveWhenAlerted) + transform.position);
        }
    }

    void GoTo(Vector3 target) {
        agent.destination = target;
    }

    void Shoot() {
        laser.startColor = Color.red;
        laser.endColor = Color.red;
        shootTimer += Time.deltaTime;
        if (shootTimer >= timeToShoot) {
            shootTimer = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX:/EShoot", transform.position);
            if (Physics.Raycast(gunPos.position, gunPos.forward, Mathf.Infinity, playerMask) && !Physics.Raycast(gunPos.position, gunPos.forward, Mathf.Infinity, environmentMask)) {
                GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<IPlayerDamage>().TakeDamage(damage, gameObject);
            }
            shooting = false;
            shootCD = true;
        }
    }

    void Explode() {
        print("Go BOOM!");
        if(Vector3.Distance(player.transform.position, transform.position) < explosionRadius) {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EExplosion", transform.position);
            player.GetComponent<IPlayerDamage>().TakeDamage(explodeDamage, gameObject);
            var pr = player.GetComponent<Rigidbody>();
            player.GetComponent<PlayerMover>().airblastin = true;
            player.GetComponent<PlayerMover>().lastInputState = PlayerState.Airborne;
            pr.velocity = new Vector3(pr.velocity.x, 0, pr.velocity.z);
            pr.AddForce((pr.position - transform.position) * playerExplosionForce * (explosionRadius / distance), ForceMode.VelocityChange);
        }
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius, explodeMask);
        foreach(Collider ec in colliders) {
            var ecRig = ec.GetComponent<Rigidbody>();
            if(ecRig != null) {
                ecRig.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1);
            }
        }
        onExplosion.Invoke();
        Destroy(gameObject);
    }

    void ChangeColor() {
        timer += Time.deltaTime;
        var t = timer / explodeTime;
        mat.color = Color.Lerp(originalColor, changeColor, t);
        //mat.color = new Color(Mathf.Lerp(mat.color.r, changeColor.r, colorChangeSpeed * Time.deltaTime), Mathf.Lerp(mat.color.g, changeColor.g, colorChangeSpeed * Time.deltaTime), Mathf.Lerp(mat.color.b, changeColor.b, colorChangeSpeed * Time.deltaTime));
    }
    #endregion

    #region Public Methods
    public void Distract(Vector3 t) {
        es = Enemystate.Distracted;
        GoTo(t);
    }
    #endregion

    #region Senses

    bool PlayerHeard() {
        var d = Vector3.Distance(agent.transform.position, player.transform.position);
        return d < detectionDistances[(int)es];
    }

    bool PlayerSeen() {
        var pPos = player.transform.position;
        var tPos = transform.position;
        var delta = pPos - tPos;
        var len = delta.magnitude;
        return len < detectionDistances[(int)es] &&
            Vector3.Angle(transform.forward, delta) < viewAngle / 2 &&
            !Physics.Raycast(tPos, delta, len, layerMask);
    }

    bool Proximity() {
        return Vector3.Distance(transform.position, agent.destination) < proximityDistance;
    }

    #endregion

    #region ShotReactions

    public void GotStunned() {
        es = Enemystate.Stunned;
        print("Got stunned");
    }

    public void GotDamage() {
        print("Took Damage");
        es = Enemystate.Hiding;
    }

    public void GotBlasted() {
        print("Got blasted");
        es = Enemystate.Alerted;
    }

    public void LaughAtEmptyGun() {
        es = Enemystate.Hunting;
        print("Ha, Ha, Ha... You Amateur! You are dead now!");
    }

    #endregion

    private void Update() {
        if (!aiming) {
            memES = es;
            actions[(int)es]();
            previousES = memES;
        }

        if(Input.GetKeyDown(KeyCode.O)) {
            Distract(new Vector3(10, 0, 5));
        }
        if (!explosive && gunPos != null) {
            laser.SetPosition(0, gunPos.position);
            laser2.SetPosition(0, gunPos.position);
            Physics.Raycast(gunPos.position, gunPos.forward, out hit, Mathf.Infinity, shootMask);
            laser.SetPosition(1, hit.point);
            laser2.SetPosition(1, hit.point);
            if (!shooting) {
                laser.startColor = new Color(1, 0, 0, 0.65f);
                laser.endColor = new Color(1, 0, 0, 0.45f);
            }
            if (shooting) {
                Shoot();
            }
            if (shootCD) {
                cdTimer += Time.deltaTime;
                if (cdTimer >= shootCooldown) {
                    cdTimer = 0;
                    shootCD = false;
                }
            }
        }
        if (agent.remainingDistance > explodeDistance) {
            GetComponentInChildren<Animator>().Play("RunCycle");
        } else GetComponentInChildren<Animator>().Play("Idle");
    }

    public void Death() {
        Destroy(laser);
        Destroy(laser2);
        Laserburn.release();
    }

    private void FixedUpdate() {
        if (aiming) {
            if (!PlayerSeen()) {
                aiming = false;
                es = Enemystate.Hunting;
            }
            if (Physics.Raycast(gunPos.position, gunPos.forward, Mathf.Infinity, playerMask) && !Physics.Raycast(gunPos.position, gunPos.forward, Mathf.Infinity, environmentMask)) {
                Laserburn.setParameterByName("LockOn", 1);
            } else Laserburn.setParameterByName("LockOn", 0);
            if (!shooting) {
                gunPos.forward = Vector3.Slerp(gunPos.forward, (GameObject.FindGameObjectWithTag("PlayerObject").transform.position - gunPos.position) + vectOffset, speed);
            }
            if (!shootCD && !shooting) {
                aimTimer += Time.deltaTime;
                if (aimTimer >= aimTime) {
                    aimTimer = 0;
                    shooting = true;
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if(hidingPointOffsets != null) {
            for(int i = 0; i < hidingPointOffsets.Length; i++) {
                var pos = transform.position + hidingPointOffsets[i];
                Gizmos.DrawSphere(pos, 0.25f);
            }
        }
    }
}
