using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float speed = 10;
    [SerializeField] float acceleration = 100;
    public static Player instance;
    InputAction moveInput;
    [HideInInspector]public Controls inputs;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        inputs = new Controls();
        moveInput = inputs.Player.Move;
        moveInput.Enable();
        rb = GetComponent<Rigidbody2D>();
    }
    void OnDisable()=>moveInput.Disable();
    // Update is called once per frame
    void Update()
    {
        Vector2 moveDir = inputs.Player.Move.ReadValue<Vector2>();
        rb.AddForce(moveDir * speed * Time.deltaTime * acceleration,ForceMode2D.Force);
    }
}
