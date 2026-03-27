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
    [Header("Interact")]
    public KeyReplacer textInteract;
    public static UIManagerGame instance;
    // Start is called before the first frame update
    void Awake()=>instance = this; 
    public HealthBar CreateHealthBar(HealtSystem healtSystem, string name)
    {
        HealthBar tmpBar = Instantiate(healthBar,contentSpawnHealthBars).GetComponent<HealthBar>();
        tmpBar.SetHealthBar(healtSystem,name);
        return tmpBar;
    }
    public void SetIntreractText(bool on)
    {
        if(textInteract == null)
            return;
        textInteract.gameObject.SetActive(on);
        if(on)
            textInteract.SetText();
        else
            textInteract.text = "Use (#Use#)";
    }
    public void RemoveHealthBar(HealthBar healthBar)
    {
        if(healthBar != null)
            Destroy(healthBar.gameObject);
    }
}
