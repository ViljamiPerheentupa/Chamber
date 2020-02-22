using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBehaviour : MonoBehaviour
{ 
    public enum Enemystate { Idle, Alert, Hunt, Shoot, Hide, Sneak, Stunned };

    public Enemystate es;
    Enemystate memES;
    Enemystate previousES;

    float[] detectionDistances = new float[] { 25f, 35f, 35f, 35f, 35f, 35f, 0 };

    GameObject player;
    Vector3 target;
    NavMeshAgent agent;
    LayerMask layerMask;

    float viewAngle = 90f;
    float alertDistance = .5f;

    float targetProximityDistance = .5f;

    float timer;
    float alertTime = 5f;
    float huntTime = 2.5f;
    float stunTime = 5f;
    float hideTime = 10f;

    UnityAction[] actions;

    void Start() {
        es = Enemystate.Idle;
        player = GameObject.Find("PlayerBody");
        agent = GetComponent<NavMeshAgent>();
        actions = new UnityAction[] { Idle, Alert, Hunt, Shoot, Hide, Sneak, Stunned };
        layerMask = LayerMask.GetMask(new string[] { "Environment" });
    }

    #region Actions
    void Idle() {
        if(previousES != Enemystate.Idle) {
            print("Idling");
        }

        if(PlayerSeen()) {
            es = Enemystate.Hunt;
            return;
        }

        if(PlayerHeard()) {
            es = Enemystate.Alert;
        }
    }
    void Alert() {
        if(previousES != Enemystate.Alert) {
            timer = Time.time + alertTime;
            print("Alerted");
            var pos = transform.position;
            target = (Vector3.Normalize(player.transform.position - pos) * alertDistance) + transform.position;
            agent.destination = target;
        }

        if(PlayerSeen()) {
            es = Enemystate.Hunt;
            return;
        }

        if(Time.time > timer) {
            es = Enemystate.Idle;
        }
    }
    void Hunt() {
        if(previousES != Enemystate.Hunt) {
            timer = Time.time + huntTime; ;
            print("Hunting");
        }
        if(PlayerSeen()) {
            target = player.transform.position;
            agent.destination = target;
            return;
        } else if(PlayerHeard()) {
            es = Enemystate.Alert;
            return;
        }
        if(Time.time > timer) {
            es = Enemystate.Alert;
        }
    }

    void Shoot() {
        if(previousES != Enemystate.Shoot) {
            // Shoot
            print("Shooting");
        } else {
            es = Enemystate.Alert;
        }
    }
    void Hide() {
        if(previousES != Enemystate.Hide) {
            print("Hiding");
            timer = Time.time + hideTime;
            // Scan for possible places to hide
            var pos = transform.position;
            target = pos + transform.forward * -5f;
            agent.destination = target;
        }
        if(Time.time > timer) {
            es = Enemystate.Alert;
        }
    }
    void Sneak() {
        if(previousES != Enemystate.Sneak) {
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
            es = Enemystate.Alert;
        }
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

    #endregion

    #region ShotReactions

    public void GotStunned() {
        es = Enemystate.Stunned;
        print("Got stunned");
    }

    public void GotDamage() {
        print("Took Damage");
        es = Enemystate.Hide;
    }

    public void GotBlasted() {
        print("Got blasted");
        es = Enemystate.Alert;
    }

    public void LaughAtEmptyGun() {
        es = Enemystate.Hunt;
        print("Ha, Ha, Ha... you Amateur! You are dead now!");
    }

    #endregion

    private void Update() {
        memES = es;
        actions[(int)es]();
        previousES = memES;
    }
}
