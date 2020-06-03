using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleTrigger : MonoBehaviour {
    public void TriggerSubtitle(string text) {
        SubtitleManager sm = FindObjectOfType<SubtitleManager>();
        if (sm) {
            sm.AddSubtitle(text);
        }
    }
}
