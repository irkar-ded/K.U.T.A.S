using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int currentMoney;
    [SerializeField] TextMeshProUGUI textMoney;
    Animator anim;
    public static Money instance;
    private void Start()
    {
        instance = this;
        if (textMoney != null)
        {
            textMoney.TryGetComponent(out anim);
            textMoney.text = $"{currentMoney}$";
        }
    }
    public void AddMoney(int count)
    {
        currentMoney += count;
        if(anim != null)
            anim.SetTrigger("Bounce");
        if(textMoney != null)
            textMoney.text = $"{currentMoney}$";
    }
    public void MinusMoney(int count)
    {
        currentMoney -= count;
        if(anim != null)
            anim.SetTrigger("Bounce");
        if(textMoney != null)
            textMoney.text = $"{currentMoney}$";
    }
}
