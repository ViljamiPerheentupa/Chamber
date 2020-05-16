using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatHandler : MonoBehaviour {
    public Text statusText;
    public float timeUntilHideStatus;
    private InputField inputField;

    // Start is called before the first frame update
    void Start() {
        inputField = GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update() {
        if(inputField.isFocused && inputField.text != "" && Input.GetKey(KeyCode.Return)) {
            TryCheat(inputField.text);
            inputField.text = "";
        }
    }

    void TryCheat(string text) {
        text.ToLower();
        statusText.color = new Color(0f, 1f, 0f, 1f);

        if (text == "giveshock") {
            GunContainer gc = FindObjectOfType<GunContainer>();
            if (gc) {
                gc.enabledAmmoTypes[0] = true;
                statusText.text = "Gave the player the shock bullet.";
            }
            else {
                statusText.color = new Color(1f, 0f, 0f, 1f);
                statusText.text = "Could not find the gun.";
            }
        }
        else if (text == "givemagnet") {
            GunContainer gc = FindObjectOfType<GunContainer>();
            if (gc) {
                gc.enabledAmmoTypes[1] = true;
                statusText.text = "Gave the player the magnetize bullet.";
            }
            else {
                statusText.color = new Color(1f, 0f, 0f, 1f);
                statusText.text = "Could not find the gun.";
            }
        }
        else if (text == "givetime") {
            GunContainer gc = FindObjectOfType<GunContainer>();
            if (gc) {
                gc.enabledAmmoTypes[2] = true;
                statusText.text = "Gave the player the time bullet.";
            }
            else {
                statusText.color = new Color(1f, 0f, 0f, 1f);
                statusText.text = "Could not find the gun.";
            }
        }
        else if (text == "giveair") {
            AirShotgun sh = FindObjectOfType<AirShotgun>();
            if (sh) {
                sh.isActivated = true;
                statusText.text = "Gave the player the airshotgun.";
            }
            else {
                statusText.color = new Color(1f, 0f, 0f, 1f);
                statusText.text = "Could not find the airshotgun.";
            }
        }
        else if (text == "kill") {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph) {
                ph.TakeDamage(10000f, null);
                statusText.text = "Player = Ded.";
            }
            else {
                statusText.color = new Color(1f, 0f, 0f, 1f);
                statusText.text = "Could not kill the player.";
            }
        }
        else {
            statusText.color = new Color(1f, 0f, 0f, 1f);
            statusText.text = "Invalid Command: " + text;
        }

        Invoke("HideStatus", timeUntilHideStatus);
    }

    void HideStatus() {
        statusText.text = "";
    }
}
