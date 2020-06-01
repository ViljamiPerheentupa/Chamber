using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    public bool open = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null) {
            anim = GetComponentInChildren<Animator>();
        }
        if (open) {
            anim.Play("DoorOpenIdle");
        }
    }

    public void Open() {
        if (!open) {
            anim.Play("DoorOpen");
        }
        open = true;
    }

    public void Close() {
        anim.Play("DoorClose");
        open = false;
    }

}
