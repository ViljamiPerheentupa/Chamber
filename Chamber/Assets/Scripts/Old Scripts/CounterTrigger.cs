using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CounterTrigger : MonoBehaviour
{
    public int countNeeded = 8;
    public int counted = 0;
    public UnityEvent onCounted;


    void Update()
    {
        if (counted >= countNeeded) {
            onCounted.Invoke();
        }
    }

    public void CountUp() {
        counted++;
    }

    public void CountDown() {
        counted--;
    }
}
