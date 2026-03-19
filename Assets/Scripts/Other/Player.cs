using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Values")]
    public float speed = 10;
    public static Player instance;
    Animator anim;
    InputAction moveInput;
    [HideInInspector]public Controls inputs;
    Gun gun;
    Vector2 input;
    Vector2 animMovement;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        inputs = new Controls();
        moveInput = inputs.Player.Move;
        moveInput.Enable();
        gun = GetComponentInChildren<Gun>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    void OnDisable()=>moveInput.Disable();
    // Update is called once per frame
    void Update()
    {
        input = inputs.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(input.x,0,input.y);
        rb.AddForce(moveDir * Time.deltaTime * 1000 * speed,ForceMode.Acceleration);
        GameManager.instance.mousePositionCamera.position = gun.getTargetLook();
        GameManager.instance.playerPositionCamera.position = transform.position;
        WalkAnimation();
    }
    public void WalkAnimation()
    {
        animMovement.x = Mathf.Lerp(animMovement.x,input.x,Time.deltaTime * 5);
        animMovement.y = Mathf.Lerp(animMovement.y,input.y,Time.deltaTime * 5);
        anim.SetFloat("x",animMovement.x);
        anim.SetFloat("y",animMovement.y);
    }
}
