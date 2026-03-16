using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EZ_Pooling;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIManagerGame : MonoBehaviour
{
    [Header("Health Bar:")]
    [SerializeField] GameObject healthBar;
    [SerializeField] Transform contentSpawnHealthBars;
    public static UIManagerGame instance;
    // Start is called before the first frame update
    void Awake()=>instance = this; 
    public HealthBar CreateHealthBar(HealtSystem healtSystem, string name)
    {
        HealthBar tmpBar = Instantiate(healthBar,contentSpawnHealthBars).GetComponent<HealthBar>();
        tmpBar.SetHealthBar(healtSystem,name);
        return tmpBar;
    }
    public void RemoveHealthBar(HealthBar healthBar)=>Destroy(healthBar.gameObject);
}
