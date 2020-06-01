using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationEvents : MonoBehaviour
{
    public void OpenSound() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/DoorOpen", transform.position);
    }

    public void CloseSound() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/DoorClose", transform.position);
    }
}
