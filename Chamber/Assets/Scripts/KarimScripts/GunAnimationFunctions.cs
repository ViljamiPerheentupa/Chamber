using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationFunctions : MonoBehaviour {
    private Gun gun;

    void Start() {
        GameObject gunReticle = GameObject.Find("Gunvas");
        
        if (gunReticle)
            gun = gunReticle.GetComponent<Gun>();
    }

    public void Shoot() {
        if (gun)
            gun.PullTrigger();
    }

    public void Disabled() {
        if (gun)
            gun.GetComponent<Gun>().enabled = false;
    }

    public void Enabled() {
        if (gun)
            gun.GetComponent<Gun>().enabled = true;
    }

    public void Reload() {
        if (gun)
            gun.GetComponent<Gun>().StartReloading();
    }

    public void SFXRelease() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GReloadRelease");
    }

    public void SFXInsert() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GReloadInsert");
    }

    public void SFXCylinder() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GCylinderRoll");
    }

    public void SFXReloadEnd() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GReloadClose");
    }

    public void SFXHammer() {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/GHammer");
    }
}


