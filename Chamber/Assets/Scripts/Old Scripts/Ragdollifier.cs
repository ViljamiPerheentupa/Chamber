using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdollifier : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) {
            GetComponentInChildren<Animator>().enabled = false;
            var rigs = GetComponentsInChildren<Rigidbody>();
            print(rigs.Length);
            foreach (Rigidbody rig in rigs) {
                if (rig.GetComponent<CharacterJoint>() != null) {
                    rig.GetComponent<CharacterJoint>().enableCollision = true;
                }
                rig.gameObject.layer = LayerMask.NameToLayer("Debris");
                rig.isKinematic = false;
                rig.useGravity = true;
                rig.AddForce(transform.forward * 10, ForceMode.Impulse);
            }
        }
    }
}
