using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Values")]
    public float speed = 10;
    [HideInInspector]public bool canMove = true;
    public static Player instance;
    Animator anim;
    InputAction moveInput;
    [HideInInspector]public Controls inputs;
    HealtSystem healtSystem;
    [HideInInspector] public Gun gun;
    Vector3 posCameraLimit = Vector3.zero;
    Vector2 input;
    Vector2 animMovement;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        canMove = true;
        inputs = new Controls();
        moveInput = inputs.Player.Move;
        moveInput.Enable();
        gun = GetComponentInChildren<Gun>();
        healtSystem = GetComponent<HealtSystem>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        anim.speed *= 1 + BuffManager.instance.passiveBuff.bonusSpeed * 0.15f;
    }
    void OnDisable()=>moveInput.Disable();
    // Update is called once per frame
    void Update()
    {
        if(canMove == false)
            return;
        if(gun != null)
            posCameraLimit = ConvertorValue.Clamp(gun.getTargetLook(),new Vector3(-20,0,-20),new Vector3(20,0,20));
        GameManager.instance.mousePositionCamera.position = posCameraLimit;
        GameManager.instance.playerPositionCamera.position = transform.position;
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false || healtSystem.healt <= 0)
            return;
        input = inputs.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(input.x,0,input.y);
        rb.AddForce(moveDir * Time.deltaTime * 1000 * speed,ForceMode.Acceleration);
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
