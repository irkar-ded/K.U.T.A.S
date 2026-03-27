using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] EventReference soundUse;
    [SerializeField] BuffItem item;
    Interactable interactable;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.interactText = $"Pick up {item.nameBuff} (#Use#)";
        interactable.onUse.AddListener(PickUp);
    }
    void PickUp()
    {
        BuffManager.instance.addBuff(item);
        RuntimeManager.PlayOneShot(soundUse);
        Destroy(gameObject);
    }
}
