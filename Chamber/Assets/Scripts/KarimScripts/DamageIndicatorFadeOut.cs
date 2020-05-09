using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorFadeOut : MonoBehaviour {
    public float fadeOutTime = 0.5f;
    private float spawnTime;

    void Start() {
        spawnTime = Time.time;
        Invoke("DeleteThis", fadeOutTime);
    }

    void Update() {
        transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f - (Time.time - spawnTime) / fadeOutTime);
    }
    
    void DeleteThis() {
        Destroy(gameObject);
    }
}
