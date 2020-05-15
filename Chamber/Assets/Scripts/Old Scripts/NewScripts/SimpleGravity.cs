using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGravity : MonoBehaviour
{
    Transform groundCheck;
    bool grounded = true;
    public LayerMask layerMask;
    CharacterController charC;
    RaycastHit hit;
    public bool grappling = false;
    void Start()
    {
        groundCheck = transform.Find("GroundCheck");
        charC = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!grounded && !grappling) {
            transform.position += Physics.gravity * Time.deltaTime;
            if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, Physics.gravity.magnitude * Time.fixedDeltaTime, layerMask)) {
                print("Hit ground");
                transform.position = hit.point + Vector3.up * 0.01f;
                grounded = true;
            }
        }
        if (!grappling) {
            if (Physics.CheckSphere(groundCheck.position, 0.01f, layerMask)) {
                print("touching ground");
                grounded = true;
            } else grounded = false;
        }
    }

    private void Update() {
        Debug.DrawRay(groundCheck.position, charC.velocity * charC.velocity.magnitude * Time.deltaTime, Color.red);

    }
}
