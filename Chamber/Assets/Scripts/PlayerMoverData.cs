using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoverData", menuName = "Chamber/Player/MoverData", order = 0)]
public class PlayerMoverData : ScriptableObject {
    public float crouchMoveSpeed = 0.5f;
    public float airSpeed = 0.4f;
    public float runSpeed = 1.5f;
    public float sprintSpeed = 2.5f;
    public float maxSpeed = 5.0f;
    public float jumpImpulse = 6.0f;
    public float jumpContinuationForce = 0.8f;
    public float jumpContinuationTime = 0.2f;
    public float fallMultiplier = 1.6f;
    public float maintainAirSpeedCoefficient = 0.98f;
    public float maintainSpeedCoefficient = 0.8f;
    public LayerMask floorMask = ~(1 << 9); 
    public float cameraHeight = -0.25f;
    public float standCapsuleHeight = 2.0f;
    public float crouchCapsuleHeight = 1.25f;
    public float startCrouchTime = 0.0f;
    public float crouchDelay = 0.3f;
    public AnimationCurve crouchCurve;
    public Vector2 weaponBobStretch = new Vector2(2.0f, 1.0f);
    public Vector2 weaponSwayAmount = new Vector3(15.0f, 15.0f);
    public float weaponSwayRecoverSpeed = 0.2f;
    public float standToCrouchWeaponSway = 80.0f;
    public float crouchToStandWeaponSway = 80.0f;
    public float jumpSway = 100.0f;
    public float landSwayMultiplier = 5.0f;
    public float couchBobSpeed = 6.0f;
    public float runBobSpeed = 8.0f;
    public float sprintBobSpeed = 12.0f;
    public float couchBobAmount = 2.0f;
    public float runBobAmount = 1.0f;
    public float sprintBobAmount = 2.0f;
    public float weaponBobFactorChangeSpeed = 0.2f;
    public float noclipSpeed = 10.0f;
    public float noclipSprintSpeed = 30.0f;
    public float coyoteTimeDuration = 2f;
    public float vaultWallDistance = 1f;
    public float vaultWallMaxHeight = 3.1f;
    public float vaultWallMinHeight = 0.5f;
    public float vaultWallStandDepth = 0.8f;
    public float[] vaultHeightChecks;
}