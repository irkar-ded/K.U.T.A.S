using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    [Header("Other")]
    [SerializeField] Renderer[] model;
    Color colorPlayer;
    bool alwaysInvincible;
    HealtSystem healtSystem;
    Animator anim;
    Gun gun;
    Coroutine invincibleCoroutine;
    void Start()
    {
        colorPlayer = model[0].material.GetColor("_Color");
        healtSystem = GetComponent<HealtSystem>();
        gun = GetComponentInChildren<Gun>();
        anim = GetComponentInChildren<Animator>();
        healtSystem.onTakeDamage.AddListener((Vector3) => MakePlayerInvincible());
        healtSystem.onDie.AddListener(OnDie);
    }
    void OnDie()
    {
        Destroy(gun);
        Destroy(this);
        //ShakeManager.instance.Shake(ShakeManager.ShakeType.Kill);
        anim.SetInteger("DeathAnimation",Random.value < 0.5f ? 1 : 0);
        anim.SetTrigger("Death");
    }
    public void MakeAlwaysInvincible()
    {
        StopAllCoroutines();
        alwaysInvincible = true;
        healtSystem.isInvincible = true;
    }
    public void MakePlayerInvincible()
    {
        if(alwaysInvincible)
            return;
        if(healtSystem.healt <= 0)
            return;
        if(invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(Invincible());
    }
    IEnumerator Invincible()
    {
        healtSystem.isInvincible = true;
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < model.Length;j++)
                model[j].materials[0].SetColor("_Color",i % 2 == 1 ? colorPlayer : Color.yellow);
            yield return new WaitForSeconds(0.1f);
        }            
        for(int j = 0; j < model.Length;j++)
            model[j].materials[0].SetColor("_Color",colorPlayer);
        healtSystem.isInvincible = false;
    }   
}
