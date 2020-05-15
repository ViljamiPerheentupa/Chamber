using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public void PlayFootstep() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/PFootstep");
    }
}
