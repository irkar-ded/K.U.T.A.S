//using EasyTextEffects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [SerializeField] int combo;
    TextMeshProUGUI textCombo;
    TextMeshProUGUI textInfoCombo;
    Animator animCombo;
    Animator animInfoCombo;
    public static ComboManager instance;
    //TextEffect textEffectCombo;
    //TextEffect textEffectInfoCombo;
    float timerCombo;
    float timer;
    List<string> comboList = new List<string>();
    int bonusCoin;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        /*textEffectCombo = textCombo.GetComponent<TextEffect>();
        textEffectInfoCombo = textInfoCombo.GetComponent<TextEffect>();*/
        animCombo = textCombo.GetComponent<Animator>();
        animInfoCombo = textInfoCombo.GetComponent<Animator>();
        textInfoCombo.text = "";
        textCombo.text = "";
        mat = new Material(textCombo.fontMaterial);
        textCombo.fontMaterial = mat;
        /*textEffectInfoCombo.Refresh();
        textEffectCombo.Refresh();*/
        if(HealtSystem.instance != null)
            HealtSystem.instance.onTakeDamage.AddListener(removeCombo);
    }
    private void OnEnable()
    {
        if(BuffManager.instance != null)
            bonusCoin = BuffManager.instance.passiveBuff.bonusCoin;
    }
    // Update is called once per frame
    void Update()
    {
        if (timerCombo > 0f && combo > 0 && comboList.Count <= 0)
            timerCombo -= Time.deltaTime;
        else
            timerCombo = 1.5f;
        if (timerCombo <= 0)
        {
            if(combo >= 3)
            {
                float instCombo = combo * 1.5f;
                Money.instance.AddMoney((int)instCombo + bonusCoin);
            }
            timerCombo = 1.5f;
            combo = 0;
        }
        textCombo.gameObject.SetActive(combo > 0);
        textInfoCombo.gameObject.SetActive(comboList.Count > 0 && combo > 0);
        mat.SetColor("_Color",new Color(0,0,0,(float)ConvertorValue.ConvertValue(timerCombo, 1.5f, 1)));
        mat.SetColor("_OutlineColor",new Color(0,0,0,(float)ConvertorValue.ConvertValue(timerCombo, 1.5f, 1)));
        if (timer < 1 && comboList.Count > 0)
            timer += Time.deltaTime;
        if(timer >= 1)
        {
            removeTextInfo();
            timer = 0;
        }
    }
    public string setTextInfo()
    {
        string temp="";
        foreach (string ch in comboList)
            temp += $"+{ch}\n";
        return temp != "" ? temp : "+shit";
    }
    public void removeCombo()
    {
        if (combo >= 3)
        {
            float instCombo = combo * 1.5f;
            Money.instance.AddMoney((int)instCombo + bonusCoin);
        }
        timerCombo = 0;
        combo = 0;
    }
    public void removeCombo(Vector3 test)
    {
        if (combo >= 3)
        {
            float instCombo = combo * 1.5f;
            Money.instance.AddMoney((int)instCombo + bonusCoin);
        }
        timerCombo = 0;
        combo = 0;
    }
    public void addCombo()
    {
        combo++;
        animCombo.SetTrigger("Bounce");
        timerCombo = 1.5f;
        textCombo.text = $"COMBO {combo}";
        //textEffectCombo.Refresh();
    }
    public void addTextInfo(string text,int count)
    {
        if (comboList.Count > 3)
            comboList.RemoveAt(0);
        animInfoCombo.SetTrigger("Bounce");
        comboList.Add(text);
        textInfoCombo.text = setTextInfo();
        //textEffectInfoCombo.Refresh();
        Money.instance.AddMoney((count * 4) + bonusCoin);
        if(ScoreManager.instance != null)
        ScoreManager.instance.addScore(count * 10);
        timer = 0;
    }
    public void removeTextInfo()
    {
        if(comboList.Count > 0)
            comboList.RemoveAt(0);
        textInfoCombo.text = setTextInfo();
        //textEffectInfoCombo.Refresh();
    }
}
