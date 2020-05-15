using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    bool open = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) {
            anim = GetComponentInChildren<Animator>();
        }
    }

    public void Open() {
        if (!open) {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/DoorOpen", transform.position);
            anim.Play("DoorOpen");
        }
        open = true;
    }

    public void Close() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/DoorClose", transform.position);
        anim.Play("DoorClose");
        open = false;
    }

}
