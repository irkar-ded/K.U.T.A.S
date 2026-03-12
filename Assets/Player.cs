using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float speed = 10;
    public static Player instance;
    InputAction moveInput;
    [HideInInspector]public Controls inputs;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        inputs = new Controls();
        moveInput = inputs.Player.Move;
        moveInput.Enable();
        rb = GetComponent<Rigidbody>();
    }
    void OnDisable()=>moveInput.Disable();
    // Update is called once per frame
    void Update()
    {
        Vector2 input = inputs.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(input.x,0,input.y);
        rb.AddForce(moveDir * speed,ForceMode.Acceleration);
    }
}
