using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    HealtSystem healtSystem;
    Coroutine invincibleCoroutine;
    void Start()
    {
        healtSystem = GetComponent<HealtSystem>();
        healtSystem.onTakeDamage.AddListener((Vector3) => MakePlayerInvincible());
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
