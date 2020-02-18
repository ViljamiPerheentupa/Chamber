using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum Enemystate { Idle, Hunting, Hiding, Shooting };

    public Enemystate es;
    GameObject player;
    Vector3 target;
    NavMeshAgent agent;
    float detectionDistance = 5f;
    float targetProximityDistance = 2f;

    void Start() {
        es = Enemystate.Idle;
        player = GameObject.Find("PlayerBody");
        agent = GetComponent<NavMeshAgent>();
    }

    void StartHunting() {
        target = player.transform.position;
        agent.destination = target;
        es = Enemystate.Hunting;
    }

    void StartIdling() {
        es = Enemystate.Idle;
        agent.destination = transform.position;
    }

    void Update() {
        if(Vector3.Distance(agent.transform.position, player.transform.position ) < detectionDistance) {
            StartHunting();
        }
        if (es == Enemystate.Hunting) {
            if(Vector3.Distance(agent.transform.position, target) < targetProximityDistance) {
                StartIdling();
            }
        }
    }
}
