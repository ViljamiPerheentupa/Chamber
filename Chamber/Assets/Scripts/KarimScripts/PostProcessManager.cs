using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessManager : MonoBehaviour {
    public float deadVignetteIntensity;
    public Color deadVignetteColor;
    public float deadContrast;

    // Private
    private float normalVignetteIntensity;
    private Color normalVignetteColor;
    private PlayerHealth health;
    private SplineParameter hueSat;
    private Vignette vignette;
    private FloatParameter contrast;
    private float normalContrast;

    void Start() {
        GameObject player = GameObject.Find("Player");
        if (player) {
            health = player.GetComponent<PlayerHealth>();
        }
        
        ColorGrading     colorGradingLayer     = null;

        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGradingLayer);
        volume.profile.TryGetSettings(out vignette);

        normalVignetteIntensity = vignette.intensity.value;
        normalVignetteColor = vignette.color.value;

        contrast = colorGradingLayer.contrast;
        normalContrast = contrast.value;

        hueSat = colorGradingLayer.hueVsSatCurve;
        hueSat.value.curve.AddKey(new Keyframe(0.00f, 0.5f));
        hueSat.value.curve.AddKey(new Keyframe(0.08f, 0.5f));
        hueSat.value.curve.AddKey(new Keyframe(0.92f, 0.5f));
    }

    void Update() {
        float hp = health.currentHealth / health.maximumHealth;

        vignette.intensity.value = Mathf.Lerp(deadVignetteIntensity, normalVignetteIntensity, hp);
        vignette.color.value = Color.Lerp(deadVignetteColor, normalVignetteColor, hp);

        contrast.value = Mathf.Lerp(deadContrast, normalContrast, hp);

        hp *= 0.5f;
        hueSat.value.curve.MoveKey(0, new Keyframe(0.00f, 1.0f - hp));
        hueSat.value.curve.MoveKey(1, new Keyframe(0.08f, hp));
        hueSat.value.curve.MoveKey(2, new Keyframe(0.92f, hp));
    }
}
