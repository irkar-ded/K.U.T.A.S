using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [SerializeField] EventReference walkSound;
    Animator anim;
    EventInstance footsteps;
    void Start()
    {
        anim = GetComponent<Animator>();
        footsteps = RuntimeManager.CreateInstance(walkSound);
    }
    public void playSoundWalk()
    {
        if(Mathf.Abs(anim.GetFloat("x")) < 0.1f && Mathf.Abs(anim.GetFloat("y")) < 0.1f)
            return;
        footsteps.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        footsteps.start();
    }
}
