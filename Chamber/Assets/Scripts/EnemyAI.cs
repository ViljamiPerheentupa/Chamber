using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    public enum Enemystate { Idle, Hunt, Shoot, Hide, Sneak };

    public Enemystate es;
    bool huntingStarted;
    GameObject player;
    Vector3 target;
    NavMeshAgent agent;
    float detectionDistance = 5f;
    float targetProximityDistance = 2f;
    UnityAction[] actions;
    
    void Start() {
        es = Enemystate.Idle;
        player = GameObject.Find("PlayerBody");
        agent = GetComponent<NavMeshAgent>();
        actions = new UnityAction[] { Idle, Hunt, Shoot, Hide, Sneak };
    }

    #region Actions
    void Idle() {
        if(Vector3.Distance(agent.transform.position, player.transform.position) < detectionDistance) {
            es = Enemystate.Hunt;
        }
    }
    void Hunt() {
        if(!huntingStarted) {
            target = player.transform.position;
            agent.destination = target;
        }
        if(Vector3.Distance(agent.transform.position, target) < targetProximityDistance) {
            es = Enemystate.Idle;
            agent.destination = transform.position;
            huntingStarted = false;
        }
    }
    void Shoot() {
    
    }
    void Hide() {
    
    }
    void Sneak() {
    
    }
    #endregion

    private void Update() {
        actions[(int)es]();
    }
}
