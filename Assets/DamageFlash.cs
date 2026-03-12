using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    HealtSystem healtSystem;
    MeshRenderer model;
    void Start()
    {
        model = GetComponentInChildren<MeshRenderer>();
        healtSystem = GetComponent<HealtSystem>();
        healtSystem.onTakeDamage.AddListener((Vector3) => OnTakeDamage());
    }
    public void OnTakeDamage()
    {
        if(healtSystem.healt == 0)
            return;
        StartCoroutine(AnimatedDamage());
    }
    IEnumerator AnimatedDamage()
    {
        for(int i = 0; i < 5; i++)
        {
            model.material.SetFloat("_FlashAmount", i % 2 == 1 ? 1 : 0);
            yield return new WaitForSeconds(0.1f);
        }
        if(healtSystem.healt == 0)
        {
            Color deathColor = model.material.color;
            deathColor.a = 0.5f;
            model.material.color = deathColor;
        }
    }
}
