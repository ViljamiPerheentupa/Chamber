using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Vector3 vectOffset;
    public GameObject goFollow;
    [SerializeField] float speed = 3.0f;

    private void Start() {
        //goFollow = Camera.main.gameObject;
        vectOffset = transform.position - goFollow.transform.position;
    }

    //private void LateUpdate() {
    //    rig.transform.position = goFollow.GetComponent<Rigidbody>().transform.position + vectOffset;
    //}
    private void Update() {
        transform.position = goFollow.transform.position + vectOffset;
        transform.localRotation = Quaternion.Slerp(transform.rotation, goFollow.transform.rotation, speed);
        //rig.MoveRotation(Quaternion.Slerp(rig.rotation, goFollow.GetComponent<Rigidbody>().transform.rotation, speed * Time.deltaTime));
    }


}
