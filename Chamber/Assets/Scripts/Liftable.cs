using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liftable : MonoBehaviour {
    [Tooltip("Whether or not the player can lift it.")]
    public bool isLiftable = true;
    [Tooltip("How far from the player does the object go when lifted.")]
    public float liftDistance = 1.0f;
    [Tooltip("When lifted, the object will tend to this angle.")]
    public Vector3 liftAngle;
};