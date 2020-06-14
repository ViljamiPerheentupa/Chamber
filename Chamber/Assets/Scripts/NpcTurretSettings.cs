using UnityEngine;

[CreateAssetMenu(fileName = "NpcTurretSettings", menuName = "Chamber/NPCs/TurretSettings")]
public class NpcTurretSettings : ScriptableObject {
    public DecalData bulletDecal;
    public float bulletForce = 20.0f;
    public float playerDetectRange = 10.0f;
    public float playerLoseRange = 12.0f;
    public float detectConeAngle = 45.0f;
    // public float playerDetectTime = 1.0f;
    public float playerForgetTime = 4.0f;
    public float bulletHitRange = Mathf.Infinity;
    public float minimumBulletDamage = 10.0f;
    public float maximumBulletDamage = 50.0f;
    public float minimumBulletDelay = 1.0f;
    public float maximumBulletDelay = 0.2f;
    public float minimumBulletSpread = 30.0f;
    public float maximumBulletSpread = 5.0f;
    public float timeToMaximum = 2.0f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
    public float trackingSpeed = 0.3f;
    public float minBarrelPitch = -60.0f;
    public float maxBarrelPitch = 60.0f;
    public float turningIdleSpeed = 20.0f;
    public LayerMask layerMask;
}