using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteMyself : MonoBehaviour {
    public float deleteTime = 1f;

    void Start() {
        Invoke("DeleteMe", deleteTime);
    }

    void DeleteMe() {
        Destroy(gameObject);
    }
}
