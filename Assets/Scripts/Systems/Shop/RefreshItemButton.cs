using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using FMODUnity;


public class RefreshItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] EventReference soundBuy;
    [SerializeField] int costItem;
    [SerializeField] TextMeshProUGUI textName;
    [SerializeField] TextMeshProUGUI textDescription;
    Button buttonItem;
    Animator anim;
    CanvasGroup aboutItem;
    bool entered;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        buttonItem = GetComponent<Button>();
        aboutItem = GetComponentInChildren<CanvasGroup>();
        ShopManager.instance.onBuy.AddListener(SetItem);
    }
    public void OnPointerEnter(PointerEventData eventData)=>entered = true;
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
        textName.text = $"{"Refresher"}-<color={(activeButton ? "#78d8b7" : "#ff5470")}>{(costItem > 0 ? costItem + "$" : "FREE")}";
        textDescription.text = "Refresh all items in the shop";
    }
    public bool activeCheakItem() => Money.instance.currentMoney >= costItem;
    public void buyItem()
    {
        RuntimeManager.PlayOneShot(soundBuy);
        Money.instance.MinusMoney(costItem);
        ShopManager.instance.onBuy.Invoke();
        ScoreManager.instance.addScore(10);
        ShopManager.instance.Refresh();
    }
}
