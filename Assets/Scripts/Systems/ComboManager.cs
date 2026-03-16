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
    [SerializeField] Image comboFill;
    //Animator animCombo;
    public static ComboManager instance;
    //TextEffect textEffectCombo;
    //TextEffect textEffectInfoCombo;
    bool comboBreak;
    float timerCombo;
    int bonusCoin;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        /*textEffectCombo = textCombo.GetComponent<TextEffect>();
        textEffectInfoCombo = textInfoCombo.GetComponent<TextEffect>();*/
        //animCombo = textCombo.GetComponent<Animator>();
        //textCombo.text = "";
        //mat = new Material(textCombo.fontMaterial);
        //textCombo.fontMaterial = mat;
        //textEffectCombo.Refresh();
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
        if(comboBreak || GameManager.instance.gameIsStarted == false)
            return;
        if (timerCombo > 0f && combo > 0)
            timerCombo -= Time.deltaTime;
        comboFill.fillAmount = (float)ConvertorValue.ConvertValue(timerCombo,2.5f,1);
        if (timerCombo <= 0 && combo > 0)
            removeCombo();
        //mat.SetColor("_Color",new Color(0,0,0,(float)ConvertorValue.ConvertValue(timerCombo, 1.5f, 1)));
    }
    public void removeCombo()
    {
        if (combo >= 3)
        {
            int instCombo = combo * 10;
            Money.instance.AddMoney(instCombo + bonusCoin);
        }
        comboFill.fillAmount = 0;
        textCombo.text = $"<color=red>COMBO BREAK AT {combo}</color>";
        timerCombo = 0;
        combo = 0;
        comboBreak = true;
    }
    public void addCombo(int count)
    {
        Money.instance.AddMoney((count * 10) + bonusCoin);
        if(ScoreManager.instance != null)
            ScoreManager.instance.addScore(count * 10);
        if(comboBreak)
            return;
        combo++;
        //animCombo.SetTrigger("Bounce");
        timerCombo = 2.5f;
        textCombo.text = $"<color=black>COMBO {combo}</color>";
        textCombo.gameObject.SetActive(true);
        //textEffectCombo.Refresh();
    }
}
