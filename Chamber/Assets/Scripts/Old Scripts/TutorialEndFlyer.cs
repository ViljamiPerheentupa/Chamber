using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEndFlyer : MonoBehaviour
{
    private Rigidbody player;
    public Transform flyPoint;
    public float amount;

    private void Start() {
        player = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    public void LetFly() {
        var direction = flyPoint.position - player.transform.position;
        player.AddForce(direction * amount, ForceMode.VelocityChange);
        print("Sent player flying");
    }
}
