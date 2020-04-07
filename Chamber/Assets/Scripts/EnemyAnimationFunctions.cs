using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationFunctions : MonoBehaviour
{
    public void EFootstep() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/EFootstep", transform.position);
    }
}
