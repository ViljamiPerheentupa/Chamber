using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonEnabler : MonoBehaviour
{
    public bool visible = false;
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void FadeIn() {
        if (!visible && anim != null) {
            anim.Play("ButtonFadeIn");
            visible = true;
        }
    }

    public void FadeOut() {
        if (visible && anim != null) {
            anim.Play("ButtonFadeOut");
            visible = false;
        }
    }
    
}
