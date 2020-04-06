using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchShieldTimer : MonoBehaviour
{
    public float duration = 3;
    Animator anim;
    bool open = false;
    float timer = 0;

    public UnityEvent timerDone;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (open) {
            timer += Time.deltaTime;
            if (timer >= duration) {
                Close();
                timer = 0;
            }
        }
    }

    public void Open() {
        if (!open) {
            anim.Play("SwitchShieldOpen");
            open = true;
        }
    }

    public void Close() {
        anim.Play("SwitchShieldClose");
        timerDone.Invoke();
        open = false;
    }
}
