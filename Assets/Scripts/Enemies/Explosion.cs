using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SphereCollider sphereCollider;
    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
    }
    void OnEnable()
    {
        sphereCollider.enabled = true;
        CancelInvoke();
        Invoke("OffCollision",0.2f);
        Invoke("EndExplosion",1f);
    }
    public void OffCollision() => sphereCollider.enabled = false;
    public void EndExplosion() => gameObject.SetActive(false);
}
