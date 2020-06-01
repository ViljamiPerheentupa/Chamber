using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerGraphic : MonoBehaviour
{
    public MeshRenderer[] timers;
    public Material onMaterial;
    public Material offMaterial;

    public void TurnOn() {
        foreach(MeshRenderer mr in timers) {
            mr.GetComponent<Timer>().BeginCountdown();
        }
    }

    public void LightOn() {
        foreach (MeshRenderer mr in timers) {
            mr.material = onMaterial;
        }
    }

    public void LightOff() {
        foreach (MeshRenderer mr in timers) {
            mr.material = offMaterial;
        }
    }

    public void TurnOff() {
        foreach(MeshRenderer mr in timers) {
            mr.GetComponent<Timer>().EndCountdown();
        }
    }
}
