using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationFunctions : MonoBehaviour
{
    public void Shoot() {
        GameObject.Find("Gunvas").GetComponent<Gun>().PullTrigger();
    }

    public void Disabled() {
        GameObject.Find("Gunvas").GetComponent<Gun>().enabled = false;
    }

    public void Enabled() {
        GameObject.Find("Gunvas").GetComponent<Gun>().enabled = true;
    }

    public void Reload() {
        GameObject.Find("Gunvas").GetComponent<Gun>().StartReloading();
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


