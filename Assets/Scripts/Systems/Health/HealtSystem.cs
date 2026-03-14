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
    private void Awake()
    {
        if(transform.tag == "Player")
            instance = this;
    }
    public void TakeDamage(float damage,Vector3 posBlood)
    {
        if (isInvincible || enabled == false)
            return;
        healt -= damage;
        healt = Mathf.Clamp(healt, minHealt, maxHealt);
        onTakeDamage.Invoke(posBlood);
        if (healt <= 0)
            Die();
    }
    public void TakeDamage(float damage)
    {
        if (isInvincible)
            return;
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
}
