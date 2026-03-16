using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageOnCollision : MonoBehaviour
{
    public bool useTag = false;
    public GameObject owner;
    public float damage;
    public UnityEvent OnTakeDamage;
    [HideInInspector] public HealtSystem lastDamagedTarget;
    void Start(){}
    private void OnTriggerEnter(Collider other)
    {
        if (useTag)
        {
            if (other.gameObject.TryGetComponent(out HealtSystem hit) && (owner == null  || owner != null && owner.tag != hit.transform.tag))
            {
                hit.TakeDamage(damage, transform.position,"LOL");
                lastDamagedTarget = hit;
                OnTakeDamage.Invoke();
            }
        }
        else
        {
            if ((owner == null  || owner != null && owner != other.gameObject) && other.gameObject.TryGetComponent(out HealtSystem hit))
            {
                hit.TakeDamage(damage, transform.position,"LOL");
                lastDamagedTarget = hit;
                OnTakeDamage.Invoke();
            }
        }
    }
}
