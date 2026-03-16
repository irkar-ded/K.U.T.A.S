using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
//using EasyTextEffects;
//using FMODUnity;


public class RefreshItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[SerializeField] EventReference soundEnter;
    [SerializeField] int costItem;
    [SerializeField] Sprite activeRefreshIcon;
    [SerializeField] Sprite deactiveRefreshIcon;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textDescription;
    //TextEffect textNameEffect;
    //TextEffect textDescriptionEffect;
    Button buttonItem;
    Animator anim;
    CanvasGroup aboutItem;
    bool entered;
    Image img;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
        buttonItem = GetComponent<Button>();
        aboutItem = GetComponentInChildren<CanvasGroup>();
        //textNameEffect = textName.GetComponent<TextEffect>();
        //textDescriptionEffect = textDescription.GetComponent<TextEffect>();
        ShopManager.instance.onBuy.AddListener(setInfoItem);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        /*if (isPurched == false)
            RuntimeManager.PlayOneShot(soundEnter);*/
        entered = true;
    }
    public void OnPointerExit(PointerEventData eventData)=>entered = false;
    public void SetItem()
    {
        if(buttonItem != null)
            buttonItem.interactable = activeCheakItem();
        setInfoItem();
    }
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
