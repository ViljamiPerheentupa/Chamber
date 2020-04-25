using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverNew : MonoBehaviour
{
    float vertical;
    float horizontal;

    bool isMoving = false;
    Vector3 lastInput;

    public float movementSpeed = 4;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        if (vertical != 0 || horizontal != 0) {
            isMoving = true;
            lastInput = transform.TransformDirection(new Vector3(horizontal, 0, vertical));
        } else isMoving = false;

        if (isMoving) {
            GetComponent<CharacterController>().Move(lastInput * movementSpeed * Time.deltaTime);
        }
    }
}
