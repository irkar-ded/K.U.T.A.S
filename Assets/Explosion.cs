using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    void OnEnable()
    {
        CancelInvoke();
        Invoke("EndExplosion",0.2f);
    }
    public void EndExplosion() => gameObject.SetActive(false);
}
