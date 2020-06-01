using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventGroup : MonoBehaviour
{
    public UnityEvent[] trigger;

    public void TriggerGroup(int index) {
        trigger[index].Invoke();
    }
}
