using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SetEvent : MonoBehaviour
{
    [SerializeField] UnityEvent[] onTriggered;
    [SerializeField] bool invokeStart = false;
    [SerializeField] float timerToInvokeStart = 0;
    void Start()
    {
        if (invokeStart)
            Invoke("OnStartEvent",timerToInvokeStart);
    }
    void OnStartEvent()=>OnSetEvent(0);
    public void OnSetEvent(int id) => onTriggered[id].Invoke();
    void OnParticleSystemStopped() => onTriggered[0].Invoke();
}
