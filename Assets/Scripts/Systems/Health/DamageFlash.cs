using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] List<int> idMaterialDamage = new List<int>{0};
    [SerializeField] Color damageStandart = Color.red;
    [SerializeField] Color toxicDamage = Color.green;
    HealtSystem healtSystem;
    [SerializeField] Renderer[] model;
    [SerializeField]List<Material> materials = new List<Material>();
    void Awake()
    {
        for(int i = 0; i < model.Length; i++)
        {
            for(int j = 0; j < idMaterialDamage.Count; j++)
            {
                Material material = new Material(model[i].materials[idMaterialDamage[j]]);
                model[i].materials[j] = material;
                materials.Add(model[i].materials[j]);
            }
        }
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
                SetColorDamage(toxicDamage);
            break;
            default:
                SetColorDamage(damageStandart);
            break;
        }
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < materials.Count; j++)
                materials[j].SetFloat("_FlashAmount", i % 2);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void SetColorDamage(Color damageColor)
    {
        for(int j = 0; j < materials.Count; j++)
            materials[j].SetColor("_ColorFlash",damageColor);
    }
}
