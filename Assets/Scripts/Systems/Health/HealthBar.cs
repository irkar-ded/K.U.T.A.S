using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] Image fillBar;
    [SerializeField] Image fillHardDamageBar;
    [SerializeField] TextMeshProUGUI textName;
    HealtSystem healthSystem;
    float timerToBackHardDamage;
    public void SetHealthBar(HealtSystem health,string name)
    {
        healthSystem = health;
        fillBar.fillAmount = 1;
        fillHardDamageBar.fillAmount = 1;
        textName.text = name;
        healthSystem.onTakeDamage.AddListener((Vector3) => reloadTimer());
    }
    public void reloadTimer() => timerToBackHardDamage = 0.2f;
    void Update()
    {
        if(healthSystem == null)
        {
            UIManagerGame.instance.RemoveHealthBar(this);
            return;
        }
        float healt = (float)ConvertorValue.ConvertFromMaxToNewMax(healthSystem.healt,healthSystem.maxHealt,1);
        fillBar.fillAmount = healt;
        if(timerToBackHardDamage > 0)
            timerToBackHardDamage -= Time.deltaTime;
        else
            fillHardDamageBar.fillAmount = Mathf.Lerp(fillHardDamageBar.fillAmount,healt,Time.deltaTime * 10);
    }
}
