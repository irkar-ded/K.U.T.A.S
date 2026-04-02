using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShakeManager : MonoBehaviour
{
    public enum ShakeType
    {
        Fire,
        Kill
    }
    [SerializeField] float shakeForce = 1;
    public static ShakeManager instance;
    public CinemachineImpulseSource fireShake;
    public CinemachineImpulseSource killShake;
    void Start()=>instance = this;
    public void Shake(ShakeType shakeType)
    {
        switch (shakeType)
        {
            case ShakeType.Fire:
                fireShake.GenerateImpulseWithForce(shakeForce);
            break;
            case ShakeType.Kill:
                killShake.GenerateImpulseWithForce(shakeForce);
            break;
        }
    }
}
