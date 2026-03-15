using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using EasyTextEffects;
//using FMODUnity;


public class RefreshItem : MonoBehaviour
{
    //[SerializeField] EventReference soundEnter;
    [SerializeField] int costItem;
    [SerializeField] Sprite activeRefreshIcon;
    [SerializeField] Sprite deactiveRefreshIcon;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textDescription;
    //TextEffect textNameEffect;
    //TextEffect textDescriptionEffect;
    Animator anim;
    CanvasGroup aboutItem;
    bool entered;
    SpriteRenderer img;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<SpriteRenderer>();
        aboutItem = GetComponentInChildren<CanvasGroup>();
        //textNameEffect = textName.GetComponent<TextEffect>();
        //textDescriptionEffect = textDescription.GetComponent<TextEffect>();
        ShopManager.instance.onBuy.AddListener(setInfoItem);
    }
    public void SetItem() => setInfoItem();
    private void Update()
    {
        anim.SetBool("Selected", entered);
        aboutItem.alpha = Mathf.Lerp(aboutItem.alpha, entered ? 1 : 0, Time.deltaTime * 10);
    }
    public void setInfoItem()
    {
        bool activeButton = activeCheakItem();
        textName.text = $"{"Refresher"}-<color={(activeButton ? "green" : "red")}>{(costItem > 0 ? costItem + "$" : "FREE")}";
        textDescription.text = "I DKN";
        img.sprite = activeButton ? activeRefreshIcon : deactiveRefreshIcon;
        //textNameEffect.Refresh();
        //textDescriptionEffect.Refresh();
    }
    public bool activeCheakItem() => Money.instance.currentMoney >= costItem;
    public void buyItem()
    {
        Money.instance.MinusMoney(costItem);
        ShopManager.instance.onBuy.Invoke();
        ScoreManager.instance.addScore(10);
        ShopManager.instance.Refresh();
    }
}
