using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            FindObjectOfType<MenuBGManager>().GetComponent<MenuBGManager>().RepositionTriangle();
        }
    }
}
