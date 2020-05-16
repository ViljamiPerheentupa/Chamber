using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationFunctions : MonoBehaviour {
    private GunContainer gun;

    void Start() {
        /*GameObject gunReticle = GameObject.Find("Gunvas");
        
        if (gunReticle)
            gun = gunReticle.GetComponent<GunContainer>();*/
    }

    public void Shoot() {
        //if (gun)
        //    gun.PullTrigger();
    }

    public void Disabled() {
        //if (gun)
        //    gun.enabled = false;
    }

    public void Enabled() {
        Debug.Log("Enabled");
        //if (gun)
        //    gun.enabled = true;
    }

    public void Reload() {
        //if (gun)
        //    gun();
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


