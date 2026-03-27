using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Values")]
    public string interactText;
    [SerializeField] bool closeUsageTextAfterUse = false;
    public bool canUse = true;
    [Header("Events")]
    public UnityEvent onStartInteract;
    public UnityEvent onStartAlwaysInteract;
    public UnityEvent onLeaveInteract;
    public UnityEvent onLeaveIAlwaysInteract;
    public UnityEvent onUse;
    [HideInInspector]public bool selected;
    public void SetCanUse(bool state) => canUse = state;
    public void StartInteract()
    {
        selected = true;
        onStartAlwaysInteract.Invoke();
        if(canUse == false)
        {
            UIManagerGame.instance.SetIntreractText(false);
            return;
        }
        if (string.IsNullOrEmpty(interactText) == false)
        {
            UIManagerGame.instance.textInteract.text = interactText;
            UIManagerGame.instance.SetIntreractText(true);
        }
        onStartInteract.Invoke();
    }
    public void LeaveInteract()
    {
        selected = false;
        onLeaveIAlwaysInteract.Invoke();
        if(canUse == false)
            return;
        onLeaveInteract.Invoke();
    }
    public void Use()
    {
        if(closeUsageTextAfterUse)
            UIManagerGame.instance.SetIntreractText(false);
        if(canUse == false)
            return;
        onUse.Invoke();
    }
}
