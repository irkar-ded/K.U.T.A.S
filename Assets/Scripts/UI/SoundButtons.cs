using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButtons : MonoBehaviour
{
    [SerializeField] EventReference soundEnter;
    [SerializeField] EventReference soundClick;
    EventInstance soundInstance;
    public void onEnterSound()
    {
        soundInstance = RuntimeManager.CreateInstance(soundEnter);
        soundInstance.getPlaybackState(out PLAYBACK_STATE state);
        if(state != PLAYBACK_STATE.PLAYING)
            soundInstance.start();
    }
    public void onClickSound()
    {
        soundInstance = RuntimeManager.CreateInstance(soundClick);
        soundInstance.start();
    }
}
