using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hitmark : MonoBehaviour
{
    [SerializeField] EventReference soundHitmark;
    [SerializeField] Image[] damageMarkLines;
    [HideInInspector]public bool isOn = true;
    public static Hitmark instance;
    CanvasGroup canvasGroup;
    Image img;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        isOn = true;
        canvasGroup = GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Pause.isPaused == false && GameManager.instance.gameIsStarted && GameManager.instance.endGameState == false)
        {
            if(Cursor.visible == true || canvasGroup.alpha < 1)
            {
                SetHitmarkPosition();
                Cursor.visible = false;
                canvasGroup.alpha = 1;
            }
        }
        else if(Cursor.visible == false || canvasGroup.alpha > 0)
        {
            Cursor.visible = true;
            canvasGroup.alpha = 0;
        }
        SetHitmarkPosition();
    }
    void SetHitmarkPosition()
    {
        if(Gun.instance == null || Player.instance.transform == null)
            return;
        Vector3 targetPos = Gun.instance.getTargetLook();
        transform.position = Camera.main.WorldToScreenPoint(targetPos + (targetPos - Player.instance.transform.position).normalized);
    }
    void OnEnable()=>Cursor.visible = false;
    void OnDisable()=>Cursor.visible = true;
    //public void PlayHitmarkAnim(bool damage) => anim.SetTrigger(damage ? "Shoot" : "Damage");
    public void PlayHitmarkAnim(bool damage)
    {
        if (damage)
        {
            anim.SetTrigger("Damage");
            PlaySoundHitmark();
        }
        else
            anim.SetTrigger("Shoot");
    }
    public void SetHitmarkColor(Color color)
    {
        for(int i = 0; i < damageMarkLines.Length;i++)
            damageMarkLines[i].color = color;
    }
    public void PlaySoundHitmark() => RuntimeManager.PlayOneShot(soundHitmark, transform.position);
}
