using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRotation : MonoBehaviour
{
    [SerializeField] LayerMask surfaceToLook;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity, surfaceToLook))
        {
            Quaternion rotToLook = Quaternion.LookRotation((hit.point - transform.position).normalized);
            rotToLook.z = 0;
            rotToLook.x = 0;
            transform.rotation = rotToLook;
        }
    }
}
