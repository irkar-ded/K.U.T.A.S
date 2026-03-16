using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] Color damageStandart = Color.red;
    [SerializeField] Color toxicDamage = Color.green;
    HealtSystem healtSystem;
    MeshRenderer model;
    void Start()
    {
        model = GetComponentInChildren<MeshRenderer>();
        healtSystem = GetComponent<HealtSystem>();
        healtSystem.onTakeDamage.AddListener((Vector3) => OnTakeDamage());
        healtSystem.onDie.AddListener(() => Destroy(healtSystem));
    }
    public void OnTakeDamage()=>StartCoroutine(AnimatedDamage());
    IEnumerator AnimatedDamage()
    {
        switch (healtSystem.currentTypeDamage)
        {
            case "Toxic":
                model.material.SetColor("_ColorFlash",toxicDamage);
            break;
            default:
                model.material.SetColor("_ColorFlash",damageStandart);
            break;
        }
        for(int i = 0; i < 5; i++)
        {
            model.material.SetFloat("_FlashAmount", i % 2);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
