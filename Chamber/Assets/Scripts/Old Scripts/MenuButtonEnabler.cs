using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonEnabler : MonoBehaviour
{
    public bool visible = false;
    Animator anim;
    public EventTrigger et;
    void Start()
    {
        anim = GetComponent<Animator>();
        et.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SFXEnable() {
        et.enabled = true;
    }

    public void SFXDisable() {
        et.enabled = false;
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
