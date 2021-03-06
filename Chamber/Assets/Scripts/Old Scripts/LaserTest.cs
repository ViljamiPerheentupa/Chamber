﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTest : MonoBehaviour
{
    public GameObject line1;
    public GameObject line2;
    public LineRenderer laser;
    public LineRenderer laser2;
    public LayerMask layerMask;
    public LayerMask playerMask;
    public LayerMask environmentMask;
    RaycastHit hit;
    public Color changeColor;
    public float colorChangeSpeed = 1;
    bool changingColor = false;
    bool followingPlayer = false;
    public float speed = 0.5f;
    public Vector3 vectOffset;
    float aimTimer = 0;
    public float aimTime = 2.5f;
    float shootTimer = 0;
    public float timeToShoot = 0.5f;
    float cdTimer = 0;
    public float shootCooldown = 3;
    bool shootCD = false;
    bool shooting = false;
    public int damage = 40;

    FMOD.Studio.EventInstance Laserburn;
    void Start()
    {
        var laser1Prefab = Instantiate(line1, Vector3.zero, Quaternion.Euler(0, 0, 0));
        laser = laser1Prefab.GetComponent<LineRenderer>();
        var laser2Prefab = Instantiate(line2, Vector3.zero, Quaternion.Euler(0, 0, 0));
        laser2 = laser2Prefab.GetComponent<LineRenderer>();
        Laserburn = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/EChargeAim");
        Laserburn.start();
    }

    // Update is called once per frame
    void Update()
    {
        laser.SetPosition(0, transform.position);
        laser2.SetPosition(0, transform.position);
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask);
        laser.SetPosition(1, hit.point);
        laser2.SetPosition(1, hit.point);
        if (!shooting) {
            laser.startColor = new Color(1, 0, 0, 0.65f);
            laser.endColor = new Color(1, 0, 0, 0.45f);
        }
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
        if (Input.GetKeyDown(KeyCode.K)) {
            changingColor = true;
        }
        if (Input.GetKeyDown(KeyCode.F1) && !followingPlayer) {
            followingPlayer = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.F1) && followingPlayer) {
            followingPlayer = false;
            return;
        }
        if (shooting) {
            Shoot();
        }
        if (shootCD) {
            cdTimer += Time.deltaTime;
            if (cdTimer >= shootCooldown) {
                cdTimer = 0;
                shootCD = false;
            }
        }
    }

    private void FixedUpdate() {
        if (Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, playerMask) && !Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, environmentMask)) {
            Laserburn.setParameterByName("LockOn", 1);
        } else Laserburn.setParameterByName("LockOn", 0);
        if (followingPlayer) {
            if (!shooting) {
                transform.forward = Vector3.Slerp(transform.forward, (GameObject.FindGameObjectWithTag("PlayerObject").transform.position - transform.position) + vectOffset, speed);
            }
            if (!shootCD && !shooting) {
                aimTimer += Time.deltaTime;
                if (aimTimer >= aimTime) {
                    aimTimer = 0;
                    shooting = true;
                }
            }
        }
    }

    private void LateUpdate() {
        if (changingColor) {
            ChangeColor();
        }
    }

    void Shoot() {
        laser.startColor = Color.red;
        laser.endColor = Color.red;
        shootTimer += Time.deltaTime;
        if (shootTimer >= timeToShoot) {
            shootTimer = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX:/EShoot", transform.position);
            if (Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, playerMask) && !Physics.Raycast(transform.position, transform.forward, Mathf.Infinity, environmentMask)) {
                GameObject.FindGameObjectWithTag("PlayerObject").GetComponent<IPlayerDamage>().TakeDamage(damage, gameObject);
            }
            shooting = false;
            shootCD = true;
        }
    }

    void ChangeColor() {
        var mat = GetComponent<MeshRenderer>().material;
        mat.color = new Color(Mathf.Lerp(mat.color.r, changeColor.r, colorChangeSpeed * Time.deltaTime), Mathf.Lerp(mat.color.g, changeColor.g, colorChangeSpeed * Time.deltaTime), Mathf.Lerp(mat.color.b, changeColor.b, colorChangeSpeed * Time.deltaTime));
    }
}
