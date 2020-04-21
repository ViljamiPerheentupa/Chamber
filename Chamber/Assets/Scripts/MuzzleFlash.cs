using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public ParticleSystem muzzle;

    public void Flash() {
        muzzle.Play(true);
    }
}
