using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HealtSystem : MonoBehaviour
{
    [Header("Values")]
    public float healt;
    public float maxHealt;
    public float minHealt;
    [Header("Events and UI")]
    public UnityEvent onDie;
    public UnityEvent addHealth;
    public UnityEvent<Vector3> onTakeDamage;
    public static HealtSystem instance;
    [HideInInspector]public bool isInvincible;
    [HideInInspector]public string currentTypeDamage;
    private void Awake()
    {
        if(transform.tag == "Player")
            instance = this;
    }
    public void TakeDamage(float damage,Vector3 posBlood,string typeDamage)
    {
        if (isInvincible || enabled == false)
            return;
        currentTypeDamage = typeDamage;
        healt -= damage;
        healt = Mathf.Clamp(healt, minHealt, maxHealt);
        onTakeDamage.Invoke(posBlood);
        if (healt <= 0)
            Die();
    }
    public void TakeDamage(float damage,string typeDamage)
    {
        if (isInvincible)
            return;
        currentTypeDamage = typeDamage;
        healt -= damage;
        healt = Mathf.Clamp(healt, minHealt, maxHealt);
        if (healt <= 0)
            Die();
    }
    public void Die() => onDie.Invoke();
    public void TakeHealt(float hp)
    {
        healt += hp;
        healt = Mathf.Clamp(healt, minHealt, maxHealt);
        addHealth.Invoke();
    }
    public void DamagePoison(int countDamage,float damage) => StartCoroutine(damagePoisonCoroutine(countDamage,damage));
    IEnumerator damagePoisonCoroutine(int countDamage,float damage)
    {
        for(int i = 0; i < countDamage; i++)
        {
            yield return new WaitForSeconds(0.2f);
            TakeDamage(damage,transform.position,"Toxic");
        }
    }
}
