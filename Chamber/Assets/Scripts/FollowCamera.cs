using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Vector3 vectOffset;
    GameObject goFollow;
    [SerializeField] float speed = 3.0f;
    Rigidbody rig;

    private void Start() {
        goFollow = Camera.main.gameObject;
        if (GetComponent<Rigidbody>() != null) {
            rig = GetComponent<Rigidbody>();
        }
        vectOffset = transform.position - goFollow.transform.position;
    }

    //private void LateUpdate() {
    //    rig.transform.position = goFollow.GetComponent<Rigidbody>().transform.position + vectOffset;
    //}
    private void Update() {
        if (GetComponent<Rigidbody>() != null) {
            rig.MovePosition(goFollow.GetComponent<Rigidbody>().transform.position + vectOffset);
            rig.MoveRotation(Quaternion.Slerp(rig.transform.rotation, goFollow.GetComponent<Rigidbody>().transform.rotation, speed));
        }
        if (GetComponent<Rigidbody>() == null) {
            transform.position = goFollow.transform.position + vectOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, goFollow.transform.rotation, speed);
        }
        //rig.MoveRotation(Quaternion.Slerp(rig.rotation, goFollow.GetComponent<Rigidbody>().transform.rotation, speed * Time.deltaTime));
    }


}
