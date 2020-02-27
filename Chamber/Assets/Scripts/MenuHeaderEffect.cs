using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuHeaderEffect : MonoBehaviour
{
    string toWrite;
    TextMeshProUGUI header;
    char[] alphabet = "abcdefghjiklmnopqrstuvwxyz".ToCharArray();
    public float timeToRealLetter = 2f;
    public float timeToSwitch = 0.1f;
    float timer;
    float timer2;

    void Start()
    {
        header = GetComponent<TextMeshProUGUI>();
        toWrite = header.text;
        header.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
        while (timer >= timeToSwitch) {
            timer -= timeToSwitch;
            string newLetter = alphabet[Random.Range(0, alphabet.Length)].ToString();
            while (newLetter == header.text) {
                newLetter = alphabet[Random.Range(0, alphabet.Length)].ToString();
            }
            header.text = newLetter;
        }
        if (timer2 >= timeToRealLetter) {
            header.text = toWrite;
            this.enabled = false;
            return;
        }
    }
}
