//using EasyTextEffects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [SerializeField] int combo;
    [SerializeField] TextMeshProUGUI textCombo;
    [SerializeField] Color colorCombo;
    [SerializeField] Color colorBreak;
    [SerializeField] Image comboFill;
    public static ComboManager instance;
    bool comboBreak;
    float timerCombo;
    int bonusCoin;
    // Start is called before the first frame update
    void Awake()=>instance = this;
    void Start()
    {
        GameManager.instance.onStartLevel.AddListener(() =>
        {
            if(HealtSystem.instance != null)
                HealtSystem.instance.onTakeDamage.AddListener((Vector3) => removeCombo());
        });   
    }
    private void OnEnable()
    {
        if(BuffManager.instance != null)
            bonusCoin = BuffManager.instance.passiveBuff.bonusCoin;
    }
    public void SetupCombo()
    {
        if(BuffManager.instance != null)
            bonusCoin = BuffManager.instance.passiveBuff.bonusCoin;
        comboBreak = false;
        textCombo.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(comboBreak || GameManager.instance.gameIsStarted == false || GameManager.instance.endGameState)
            return;
        if (timerCombo > 0f && combo > 0)
            timerCombo -= Time.deltaTime;
        comboFill.fillAmount = (float)ConvertorValue.ConvertValue(timerCombo,2.5f,1);
        if (timerCombo <= 0 && combo > 0)
            removeCombo();
    }
    public void removeCombo()
    {
        if (combo >= 3)
        {
            int instCombo = combo * 15;
            Money.instance.AddMoney(instCombo + bonusCoin);
        }
        comboFill.fillAmount = 0;
        textCombo.color = colorBreak;
        textCombo.text = $"COMBO BREAK AT {combo}";
        timerCombo = 0;
        combo = 0;
        comboBreak = true;
    }
    public void addCombo(int count)
    {
        Money.instance.AddMoney((count * 14) + bonusCoin);
        if(ScoreManager.instance != null)
            ScoreManager.instance.addScore(count * 10);
        if(comboBreak)
            return;
        combo++;
        timerCombo = 2.5f;
        textCombo.color = colorBreak;
        textCombo.text = $"<color=black>COMBO {combo}</color>";
        textCombo.gameObject.SetActive(true);
    }
}
