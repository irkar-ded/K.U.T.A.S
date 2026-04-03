using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [SerializeField] EventReference walkSound;
    EventInstance footsteps;
    void Start()=>footsteps = RuntimeManager.CreateInstance(walkSound);
    public void playSoundWalk()
    {
        footsteps.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        footsteps.start();
    }
}
