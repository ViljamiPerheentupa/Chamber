using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISendMessage : MonoBehaviour
{
    public string message;
    [Tooltip("Leaving this at 0 will use the default message duration")]
    public float duration;

    public void SendMessage() {
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<IUIMessage>().UIMessage(message, duration);
    }
}
