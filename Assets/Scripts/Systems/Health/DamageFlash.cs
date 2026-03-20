using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] int idMaterialDamage;
    [SerializeField] Color damageStandart = Color.red;
    [SerializeField] Color toxicDamage = Color.green;
    HealtSystem healtSystem;
    [SerializeField] Renderer[] model;
    void Start()
    {
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
                for(int i = 0; i < model.Length;i++)
                    model[i].materials[idMaterialDamage].SetColor("_ColorFlash",toxicDamage);
            break;
            default:
                for(int i = 0; i < model.Length;i++)
                    model[i].materials[idMaterialDamage].SetColor("_ColorFlash",damageStandart);
            break;
        }
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < model.Length;j++)
                model[j].materials[idMaterialDamage].SetFloat("_FlashAmount", i % 2);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
