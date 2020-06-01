using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent onTimer;
    public int duration;
    float timer;
    public bool counting;
    public int secondsPassed;
    public string FmodEvent;

    void Update()
    {
        if (counting) {
            timer += Time.deltaTime;
            if (timer >= 1) {
                secondsPassed++;
                TimerSound();
                timer -= 1;
            }
            if (secondsPassed >= duration) {
                counting = false;
                secondsPassed = 0;
                onTimer.Invoke();
            }
        }
    }

    void TimerSound() {
        if (FmodEvent != "")
        FMODUnity.RuntimeManager.PlayOneShot(FmodEvent, transform.position);
    }

    public void BeginCountdown() {
        ResetCountdown();
        counting = true;
    }

    public void ResetCountdown() {
        timer = 0;
        secondsPassed = 0;
    }

    public void EndCountdown() {
        ResetCountdown();
        counting = false;
    }

    public void EndAndTriggerCountdown() {
        ResetCountdown();
        counting = false;
        onTimer.Invoke();
    }
}
