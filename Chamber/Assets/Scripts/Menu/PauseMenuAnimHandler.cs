using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuAnimHandler : MonoBehaviour {
    public Image background;
    public List<CanvasGroup> animList = new List<CanvasGroup>();
    public float delay = 0.2f;
    public float animDuration = 0.8f;

    public void StartFade(bool isFadingIn) {
        if (isFadingIn) gameObject.SetActive(true);
        StartCoroutine(Fade(isFadingIn));
    }

    public IEnumerator Fade(bool isFadingIn) {
        float startFade = Time.realtimeSinceStartup;
        float menuTotalTime = animDuration + delay * animList.Count;
        float menuFinishTime = startFade + menuTotalTime;

        Color backgroundColor = background.color;

        while (Time.realtimeSinceStartup < menuFinishTime) {
            float bgt = (Time.realtimeSinceStartup - startFade) / menuTotalTime;
            if (!isFadingIn) bgt = 1 - bgt;
            backgroundColor.a = 0.96f * bgt;
            background.color = backgroundColor;

            for (int i = 0; i < animList.Count; ++i) {
                float t = (Time.realtimeSinceStartup - (startFade + i * delay)) / animDuration;
                t = Mathf.Clamp(t, 0f, 1f);
                if (!isFadingIn) t = 1 - t;

                float scale = Mathf.Lerp(1.5f, 1.0f, t);
                animList[i].transform.localScale = new Vector3(scale, scale, 1);
                animList[i].alpha = t;
            }
            yield return null;
        }
        
        if (isFadingIn) {
            backgroundColor.a = 0.96f;
            background.color = backgroundColor;
            for (int i = 0; i < animList.Count; ++i) {
                animList[i].transform.localScale = new Vector3(1f, 1f, 1f);
                animList[i].alpha = 1f;
            }
        }
        
        if (!isFadingIn) gameObject.SetActive(false);
    }
}
