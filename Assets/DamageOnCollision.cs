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
    void Start(){}
    private void OnTriggerEnter(Collider other)
    {
        if (useTag)
        {
            if (other.gameObject.TryGetComponent<HealtSystem>(out HealtSystem hit) && owner.tag != hit.transform.tag)
            {
                hit.TakeDamage(damage, transform.position);
                OnTakeDamage.Invoke();
            }
        }
        else
        {
            if (owner != other.gameObject && other.gameObject.TryGetComponent<HealtSystem>(out HealtSystem hit))
            {
                hit.TakeDamage(damage, transform.position);
                OnTakeDamage.Invoke();
            }
        }
    }
}
