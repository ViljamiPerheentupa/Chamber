using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class MenuEffects : MonoBehaviour
{
    PostProcessVolume pp;
    ChromaticAberration ca;
    Vignette vg;
    float intensity = 0.05f;
    float maxIntensity = 0.35f;
    float minIntensity = 0.02f;
    bool goingDownCA = false;
    float speed = 0.001f;
    float vignetteDefault;
    void Start()
    {
        pp = GetComponent<PostProcessVolume>();
        pp.profile.TryGetSettings(out ca);
        pp.profile.TryGetSettings(out vg);
        vignetteDefault = vg.intensity.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (goingDownCA) {
            intensity = Mathf.Lerp(intensity, minIntensity, speed);
            if (intensity <= minIntensity + 0.05f) {
                goingDownCA = false;
            }
        }
        if (!goingDownCA) {
            intensity = Mathf.Lerp(intensity, maxIntensity, speed);
            if (intensity >= maxIntensity - 0.05f) {
                goingDownCA = true;
            }
        }
        ca.intensity.value = intensity;
        vg.intensity.value = vignetteDefault + intensity / 4;
    }
}
