using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    public enum ObjectiveType { CountDown, CountUp }
    public ObjectiveType objectiveType;
    public int countNeeded;
    int currentCount;

    public UnityEvent onCountReached;
    void Start()
    {
        if(objectiveType == ObjectiveType.CountDown) {
            currentCount = countNeeded;
            countNeeded = 0;
        }
        if (objectiveType == ObjectiveType.CountUp) {
            currentCount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCount == countNeeded) {
            onCountReached.Invoke();
        }
    }

    public void CountUp() {
        currentCount++;
    }

    public void CountDown() {
        currentCount--;
    }

    public void ObjectiveTest() {
        print("You done did it, champ!");
    }
}
