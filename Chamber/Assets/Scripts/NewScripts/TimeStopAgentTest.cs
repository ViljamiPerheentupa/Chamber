using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TimeStopAgentTest : MonoBehaviour
{
    public bool active = false;
    NavMeshAgent agent;
    Animator anim;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = transform.position;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            active = active ? false : true;
        }

        if (active) {
            agent.destination = Camera.main.gameObject.transform.position;
        } else agent.destination = transform.position;

        if (!GetComponent<TimeStopperTest>().timeLocked) {
            if (agent.remainingDistance > 0.5f) {
                anim.Play("RunCycle");
            } else anim.Play("Idle");
        }
    }
}
