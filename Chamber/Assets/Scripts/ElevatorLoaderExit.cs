using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLoaderExit : MonoBehaviour {
    public ElevatorLoader elevatorLoader;

    private void OnTriggerExit(Collider other) {
        elevatorLoader.LeftElevator();
    }
}
