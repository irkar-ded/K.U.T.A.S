using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using FMODUnity;

public class ShopItemHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textDescription;
    Button buttonItem;
    CanvasGroup aboutItem;
    [HideInInspector]public ShopItem item;
    [SerializeField] EventReference soundBuy;
    Animator anim;
    bool entered;
    [HideInInspector]public bool isPurched;
    Image img;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        buttonItem = GetComponent<Button>();
        aboutItem = GetComponentInChildren<CanvasGroup>();
        if(ShopManager.instance != null)
            ShopManager.instance.onBuy.AddListener(CheakStateItem);
    }
    private void Update()
    {
        if(item == null)
            return;
        anim.SetBool("Selected", isPurched == false && entered);
        aboutItem.alpha = Mathf.Lerp(aboutItem.alpha, isPurched == false && entered ? 1 : 0, Time.deltaTime * 10);
    }
    public void OnPointerEnter(PointerEventData eventData)=>entered = true;
    public void OnPointerExit(PointerEventData eventData)=>entered = false;
    public void SetItemHolder(ShopItem shopItem)
    {
        item = shopItem;
        CheakStateItem();
    }
    public void CheakStateItem()
    {
        if(buttonItem != null)
            buttonItem.interactable = activeCheakButton();
        setInfoItem();
    }
    public void setInfoItem()
    {
        if(item == null)
            return;
        if (textName != null)
            textName.text = $"{item.nameItem}-<color={(Money.instance != null && Money.instance.currentMoney >= item.costItem || Money.instance == null ? "#78d8b7" : "#ff5470")}>{(item.costItem > 0 && Money.instance != null ? item.costItem + "$" : "FREE")}";
        if (textDescription != null)
            textDescription.text = getTextDescription();
        if (img != null)
            img.sprite = item.iconItem;
    }
    public bool activeCheakButton()
    {
        if (Money.instance != null)
            return isPurched == false && Money.instance.currentMoney >= item.costItem;
        else
            return true;
    }
    public string getTextDescription()
    {
        string tempText = "";
        if(item.description != "")
            tempText = item.description;
        if (tempText != "")
            tempText += $"\n";
        if (item.buff.passiveBuff.damage != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.damage, false, false)} {"Damage"}\n";
        if (item.buff.passiveBuff.bonusSpeed != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.bonusSpeed, false, false)} {"Speed"}\n";
        if (item.buff.passiveBuff.bonusForceBullet != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.bonusForceBullet, false, false)} {"Force Bullet"}\n";
        if (item.buff.passiveBuff.bounceBullet != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.bounceBullet, false, false)} {"Bounce Bullet"}\n";
        if (item.buff.passiveBuff.toxicBullet != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.toxicBullet, false, false)} {"Toxic Bullet"}\n";
        if (item.buff.passiveBuff.xRayBullet != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.xRayBullet, false, false)} {"X-Ray Bullet"}\n";
        if (item.buff.passiveBuff.maxHealth != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.maxHealth, false, false)} {"Max Health"}\n";
        if (item.buff.passiveBuff.fireRate != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.fireRate, false, false)} {"Fire Rate"}\n";
        if (item.buff.passiveBuff.bonusCoin != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.bonusCoin, false, false)} {"Additional Income"}\n";
        if (item.buff.passiveBuff.lessTimeFade != 0)
            tempText += $"{numberToText(item.buff.passiveBuff.lessTimeFade, true, false)} {"Less Time Fades"}\n";
        return tempText;
    }
    public string getColorNumber(float number, bool invert)
    {
        if (invert == false)
            return number > 0 ? "#78d8b7" : "#ff5470";
        else
            return number > 0 ? "#ff5470" : "#78d8b7";
    }
    public string numberToText(float number, bool invert, bool chance)
    {
        string temp = $"<color={getColorNumber(number, invert)}>";
        if (number > 0)
            temp += "+";
        if (chance)
            temp += $"{number}%</color>";
        else
            temp += $"{number}</color>";
        return temp;
    }
    public void buyItem()
    {
        if (Money.instance != null)
        {
            Money.instance.MinusMoney(item.costItem);
            isPurched = true;
        }
        BuffManager.instance.addBuff(item.buff);
        if (ShopManager.instance != null)
        {
            ShopManager.instance.onBuy.Invoke();
            ShopManager.instance.DropCountItem(item.nameItem);
        }
        if(ScoreManager.instance != null)
            ScoreManager.instance.addScore(10);
        RuntimeManager.PlayOneShot(soundBuy);
        CheakStateItem();
    }
}
