using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGlitcher : MonoBehaviour
{
    public bool glitching = false;
    float duration;
    public float minDuration = 0.4f;
    public float maxDuration = 1f;
    Quaternion normalRotation;
    float cooldown;
    public float minCooldown = 45;
    public float maxCooldown = 240;
    float glitchTimer = 0;
    float timer = 0;
    float cooldownTimer = 0;
    Vector3 targetRotation;
    public float minRotation = -15;
    public float maxRotation = 15;
    public AnimationCurve curve;
    float glitchStartTime;
    void Start()
    {
        cooldown = Random.Range(20, maxCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) StartGlitch();
        if (!glitching) {
            timer += Time.deltaTime;
            if (timer >= cooldown) {
                StartGlitch();
            }
        } else Glitch();
    }

    void StartGlitch() {
        normalRotation = transform.rotation;
        cooldown = Random.Range(minCooldown, maxCooldown);
        duration = Random.Range(minDuration, maxDuration);
        glitching = true;
        timer = 0;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(minRotation, maxRotation) + transform.rotation.eulerAngles.z);
        targetRotation = new Vector3(0, 0, Random.Range(minRotation, maxRotation) + transform.rotation.eulerAngles.z);
        glitchStartTime = Time.time;
    }

    void EndGlitch() {
        glitching = false;
        transform.rotation = normalRotation;
        glitchTimer = 0;
    }

    void Glitch() {
        glitchTimer += Time.deltaTime;
        var currentRotation = transform.rotation.eulerAngles;
        float t = Time.time - glitchStartTime;
        t = curve.Evaluate(t);
        if (glitchTimer < duration) {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(currentRotation, targetRotation, t));
        } else if (glitchTimer >= duration) {
            EndGlitch();
        }
    }
}
