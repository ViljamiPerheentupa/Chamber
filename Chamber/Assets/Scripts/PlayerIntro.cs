using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntro : MonoBehaviour
{
    public GameObject player;
    public GameObject gunvas;
    public void ReleaseCamera() {
        player.SetActive(true);
        gunvas.SetActive(true);
        Destroy(gameObject);
    }
}
