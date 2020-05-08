using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessManager : MonoBehaviour {
    public float deadVignetteIntensity;
    public Color deadVignetteColor;
    public float deadContrast;

    // Private
    private float normalVignetteIntensity;
    private Color normalVignetteColor;
    private PlayerHealth health;
    private TextureCurveParameter hueVsSat;
    private Vignette vignette;
    //private FloatParameter contrast;
    private float normalContrast;

    void Start() {
        GameObject player = GameObject.Find("Player");
        if (player) {
            health = player.GetComponent<PlayerHealth>();
        }
        
        ColorCurves colorSplines     = null;

        Volume volume = gameObject.GetComponent<Volume>();
        volume.profile.TryGet<ColorCurves>(out colorSplines);
        volume.profile.TryGet<Vignette>(out vignette);

        normalVignetteIntensity = vignette.intensity.value;
        normalVignetteColor = vignette.color.value;

        //normalContrast = contrast.value;

        hueVsSat = colorSplines.hueVsSat;
        hueVsSat.value.AddKey(0.00f, 0.5f);
        hueVsSat.value.AddKey(0.08f, 0.5f);
        hueVsSat.value.AddKey(0.92f, 0.5f);
    }

    void Update() {
        float hp = health.currentHealth / health.maximumHealth;
        
        ColorCurves colorSplines     = null;
        
        Volume volume = gameObject.GetComponent<Volume>();
        volume.profile.TryGet<ColorCurves>(out colorSplines);
        volume.profile.TryGet<Vignette>(out vignette);

        vignette.intensity.value = Mathf.Lerp(deadVignetteIntensity, normalVignetteIntensity, hp);
        vignette.color.value = Color.Lerp(deadVignetteColor, normalVignetteColor, hp);

        //contrast.value = Mathf.Lerp(deadContrast, normalContrast, hp);

        hueVsSat = colorSplines.hueVsSat;
        hueVsSat.value.MoveKey(0, new Keyframe(0.00f, 1.0f - 0.4f * hp));
        hueVsSat.value.MoveKey(1, new Keyframe(0.08f, 0.6f * hp));
        hueVsSat.value.MoveKey(2, new Keyframe(0.92f, 0.6f * hp));
    }
}
