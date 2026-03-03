using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestRotation : MonoBehaviour
{
    InputAction mouseInput;
    // Start is called before the first frame update
    void Start()
    {
        mouseInput = Player.instance.inputs.Player.Look;
        mouseInput.Enable();
    }
    void OnDisable()=>mouseInput.Disable();
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos = mousePos - transform.position;
        mousePos = mousePos.normalized;
        float rot = Mathf.Atan2(mousePos.y, mousePos.x) * (180/Mathf.PI);
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }
}
