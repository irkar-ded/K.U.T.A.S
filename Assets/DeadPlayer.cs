using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    HealtSystem healtSystem;
    Animator anim;
    Gun gun;
    Coroutine invincibleCoroutine;
    void Start()
    {
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
        anim.SetInteger("DeathAnimation",Random.value < 0.5f ? 1 : 0);
        anim.SetTrigger("Death");
    }
    public void MakePlayerInvincible()
    {
        if(invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(Invincible());
    }
    IEnumerator Invincible()
    {
        healtSystem.isInvincible = true;
        yield return new WaitForSeconds(1);
        healtSystem.isInvincible = false;
    }
}
