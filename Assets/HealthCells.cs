using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCells : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject healthCellPrefab;
    [SerializeField] Transform healthCellsContent;
    HealtSystem currentHealtSystem;
    List<GameObject> healthCellsFill = new List<GameObject>();
    public static HealthCells instance;
    void Awake()=>instance = this;
    public void SetupHealthCells(HealtSystem healtSystem)
    {
        currentHealtSystem = healtSystem;
        currentHealtSystem.onTakeDamage.AddListener((Vector3) => OnTakeDamage());
        while(healthCellsFill.Count < currentHealtSystem.maxHealt)
        {
            GameObject healthCellFill = Instantiate(healthCellPrefab,healthCellsContent).transform.Find("HealthFill").gameObject;
            healthCellsFill.Add(healthCellFill);
        }
        for(int i = 0; i < currentHealtSystem.maxHealt;i++)
            healthCellsFill[i].SetActive(true);
    }
    public void OnTakeDamage()
    {
        for(int i = 0;i < currentHealtSystem.maxHealt;i++)
        {
            if(i >= currentHealtSystem.healt)
                healthCellsFill[i].SetActive(false);
        }
    }
}
