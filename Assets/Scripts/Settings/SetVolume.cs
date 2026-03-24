using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using FMOD.Studio;
using FMODUnity;
using TMPro;

public class SetVolume : MonoBehaviour
{
    [SerializeField] EventReference soundReference;
    [SerializeField] string saveIndex;
    [SerializeField] string pathBus;
    EventInstance soundInstance;
    Bus audioLayer;
    Slider slider;
    public void PrepareSettings()
    {
        audioLayer = RuntimeManager.GetBus(pathBus);
        slider = GetComponent<Slider>();
        float volume = SettingsManager.instance.getVolume(saveIndex);
        slider.value = volume;
        audioLayer.setVolume(volume);
        if (soundReference.IsNull == false)
            soundInstance = RuntimeManager.CreateInstance(soundReference);
    }
    public void ChangeValue()
    {
        float newVolume = slider.value;
        SettingsManager.instance.SaveVolume(saveIndex, newVolume);
        audioLayer.setVolume(newVolume);
        if (soundReference.IsNull == false)
        {
            soundInstance.getPlaybackState(out PLAYBACK_STATE state);
            if(state != PLAYBACK_STATE.PLAYING)
                soundInstance.start();
        }
        audioLayer.setVolume(newVolume);
    }
}
