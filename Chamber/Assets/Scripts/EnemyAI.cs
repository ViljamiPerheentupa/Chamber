using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public enum Enemystate { Idle, Alert, Hunt, Shoot, Hide, Sneak }; // Todo: Alert?

    public Enemystate es;
    
    bool idling;
    bool alerted;
    bool hunting;
    bool shooting;
    bool hiding;
    bool sneaking;

    GameObject player;
    Vector3 target;
    NavMeshAgent agent;

    float idleDetectionDistance = 15f;
    float alertDetectionDistance = 25f;
    float shootingDistance = 10f;
    float targetProximityDistance = 5f;

    float alertTimer = 5f;

    UnityAction[] actions;
    
    void Start() {
        es = Enemystate.Idle;
        player = GameObject.Find("PlayerBody");
        agent = GetComponent<NavMeshAgent>();
        actions = new UnityAction[] { Idle, Alert, Hunt, Shoot, Hide, Sneak }; // Todo: Alert?
    }

    #region Actions
    void Idle() {
        if(!idling) {
            idling = true;
            print("Idling");
        }
        if(Vector3.Distance(agent.transform.position, player.transform.position) < idleDetectionDistance) {
            es = Enemystate.Hunt;
            idling = false;
        }
    }
    void Alert() {
        if(!alerted) {
            alertTimer = Time.time + 5f;
            alerted = true;
            print("Alerted");
        }

        if(Time.time > alertTimer) {
            es = Enemystate.Idle;
            alerted = false;
            // Todo: Return to original post maybe?
        }

        if(Vector3.Distance(agent.transform.position, player.transform.position) < alertDetectionDistance) {
            es = Enemystate.Hunt;
            alerted = false;
        }

        // Todo: Small area patrol
    }
    void Hunt() {
        if(!hunting) {
            target = player.transform.position;
            agent.destination = target;
            hunting = true;
            print("Hunting");
        } //Todo: think logic below
        if(Vector3.Distance(agent.transform.position, target) < targetProximityDistance) {
            if(Vector3.Distance(agent.transform.position, player.transform.position) > alertDetectionDistance) {
                es = Enemystate.Alert;
                agent.destination = transform.position;
                hunting = false;
            } else if (Vector3.Distance(agent.transform.position, player.transform.position) > shootingDistance){
                es = Enemystate.Shoot;
                hunting = false;
            } else {
                target = player.transform.position;
                agent.destination = target;
            }
        }
    }
    void Shoot() {
        if(!shooting) {
            shooting = true;
            print("Shooting");
        } else {
            es = Enemystate.Alert;
            shooting = false;
        }
    }
    void Hide() {
        if(!hiding) {
            hiding = true;
            print("Hiding");
        }
    }
    void Sneak() {
        if(!sneaking) {
            sneaking = true;
            print("sneaking");
        }
    }
    #endregion

    private void Update() {
        actions[(int)es]();
    }
}
