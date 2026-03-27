using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] LayerMask layerInteract;
    [SerializeField] float distance = 10;
    InputAction useKey;
    Controls gameInputs;
    Transform currentItem;
    void OnEnable()
    {
        if (SettingsManager.instance != null)
            gameInputs = SettingsManager.gameInputs;
        else
            gameInputs = new Controls();
        useKey = gameInputs.Player.Use;
        useKey.Enable();
    }
    void OnDisable()
    {
        useKey.Disable();
        LeaveInteract();
    }
    void Update()
    {
        if(Pause.isPaused == true)
        {
            LeaveInteract();
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, layerInteract);
        if(colliders.Length > 0)
        {
            if(currentItem != null && useKey.WasPressedThisFrame())
            {
                currentItem.SendMessage("Use",SendMessageOptions.DontRequireReceiver);
                print("Use");
            }
            if(currentItem == colliders[0].transform)
                return;
            if(currentItem != null)
            {
                currentItem.gameObject.SendMessage("LeaveInteract",SendMessageOptions.DontRequireReceiver);
                print("LeaveInteract");
            }
            currentItem = colliders[0].transform;
            UIManagerGame.instance.SetIntreractText(true);
            print($"StartInteract:{currentItem.name}");
            currentItem.SendMessage("StartInteract",SendMessageOptions.DontRequireReceiver);
        }
        else
            LeaveInteract();
    }
    public void LeaveInteract()
    {
        if(currentItem != null)
        {
            currentItem.gameObject.SendMessage("LeaveInteract",SendMessageOptions.DontRequireReceiver);
            UIManagerGame.instance.SetIntreractText(false);
            currentItem = null;
            print("LeaveInteract");
        }
    }
}
