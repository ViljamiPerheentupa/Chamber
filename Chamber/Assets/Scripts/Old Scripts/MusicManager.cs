using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public bool playOnStart = true;
    FMOD.Studio.EventInstance Music;
    public string musicPath = "event:/Music/Tutorial";
    public string parameter1 = "Sturcture";
    public string parameter2 = "Threatlevel";
    public int mvalue;
    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(musicPath);
        if (playOnStart) {
            SetParameter1(0);
            StartMusic();
        }
    }

    public void StartMusic() {
        Music.start();
        Music.release();
    }

    public void StopMusic() {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetParameter1(int value) {
        Music.setParameterByName(parameter1, value);
        print("Set parameter " + parameter1 + " to " + value);
    }

    public void SetParameter2(int value) {
        Music.setParameterByName(parameter2, value);
    }

    public void SetParameter(string parameter, int value) {
        Music.setParameterByName(parameter, value);
        mvalue = value;
    }
}
