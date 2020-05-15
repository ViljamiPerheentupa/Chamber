using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    Animator anim;
    public string animationClip;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayClip() {
        anim.Play(animationClip);
    }

}
