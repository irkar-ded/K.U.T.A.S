using System.Collections.Generic;
using System.Linq;
using EZ_Pooling;
//using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class ShopManager : MonoBehaviour
{
    public class ShopItemTemp {
        public string nameItem;
        public int countBuyedItem;
        public ShopItemTemp(string name)
        {
            nameItem = name;
            countBuyedItem = 0;
        }
    }
    //[SerializeField] EventReference soundExit;
    [SerializeField] RefreshItem itemRefresh;
    [SerializeField] ShopItemHolder itemShopHolder;
    [SerializeField] Transform contentItems;
    [SerializeField] ShopItem[] itemsShop;
    [SerializeField] ShopItem[] customItems;
    List<ShopItemTemp> itemsShopNames = new List<ShopItemTemp>();
    List<ShopItemHolder> currentItems = new List<ShopItemHolder>();
    public static ShopManager instance;
    [SerializeField] public UnityEvent onBuy;
    Controls gameInputs;
    InputAction pauseKey;
    RefreshItem refreshItem;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        for (int i = 0; i < itemsShop.Length; i++)itemsShopNames.Add(new ShopItemTemp(itemsShop[i].nameItem));
        onBuy.AddListener(() =>
        {
            if(ScoreManager.instance != null)
                ScoreManager.instance.addScore(10); 
        });
        if (SettingsManager.instance != null)
            gameInputs = SettingsManager.gameInputs;
        else
            gameInputs = new Controls();
        pauseKey = gameInputs.Player.Pause;
        pauseKey.Enable();
    }
    void Update()
    {
        if (pauseKey.WasPerformedThisFrame())
        {
            gameObject.SetActive(false);
            GameManager.instance.NextStage();
        }
    }
    void OnEnable()
    {
        Pause.canPause = false;
        createItems();
    }
    public void createItems()
    {
        //bool isOutStock = false;
        if (customItems.Length > 0)
        {
           /* if (canSpawnItemsCustom(customItems.ToList()).Count <= 0)
                isOutStock = true;
            else
            {*/
                for(int j = 0;j < customItems.Length; j++)
                {
                    ShopItemHolder randomItem = Instantiate(itemShopHolder, contentItems);
                    randomItem.SetItemHolder(canSpawnItemsCustom(customItems.ToList())[Mathf.Clamp(j, 0, canSpawnItemsCustom(customItems.ToList()).Count - 1)]);
                    currentItems.Add(randomItem);
                }
            //}
        }
        else
        {
            /*if (canSpawnItems().Count <= 0)
                isOutStock = true;
            else
            {*/
                for(int i = 0; i < 3; i++)
                {
                    List<ShopItem> tempItems = canSpawnItems();
                    ShopItemHolder randomItem = Instantiate(itemShopHolder, contentItems);
                    randomItem.SetItemHolder(tempItems[Random.Range(0, tempItems.Count)]);
                    currentItems.Add(randomItem);
                }
            //}
        }
        /*if (isOutStock == true)
        {
            dialoge.onDialoge("Out stock");
            print("dadwaefgwafesaf");
            return;
        }*/
        refreshItem = Instantiate(itemRefresh, contentItems);
        refreshItem.SetItem();
    }
    public void Refresh()
    {
        destroyItems();
        createItems();
    }
    private void OnDisable()
    {
        Pause.canPause = true;
        destroyItems();
    }
    public void destroyItems()
    {
        foreach (ShopItemHolder shopButton in currentItems)
        {
            if(shopButton != null)
                Destroy(shopButton.gameObject);
        }
        currentItems.Clear();
        if(refreshItem != null)
            Destroy(refreshItem.gameObject);
    }
    public List<ShopItem> canSpawnItems()
    {
        HashSet<string> usedNames = new HashSet<string>();
        foreach (ShopItemHolder itemCurrent in currentItems)
            usedNames.Add(itemCurrent.item.nameItem);
        foreach (ShopItemTemp itemCurrent in itemsShopNames)
        {
            ShopItem shopItem = itemsShop.First(x=> x.nameItem == itemCurrent.nameItem);
            if (itemCurrent.countBuyedItem >= shopItem.maxCountItemsBuyed && shopItem.maxCountItemsBuyed != -1)
                usedNames.Add(itemCurrent.nameItem);
        }
        List<ShopItem> result = new List<ShopItem>();
        foreach (ShopItem item in itemsShop)
        {
            if (!usedNames.Contains(item.nameItem))
                result.Add(item);
        }
        return result;
    }
    public List<ShopItem> canSpawnItemsCustom(List<ShopItem> customItems)
    {
        HashSet<string> usedNames = new HashSet<string>();
        foreach (ShopItemTemp itemCurrent in itemsShopNames)
        {
            ShopItem shopItem = itemsShop.First(x => x.nameItem == itemCurrent.nameItem);
            if (itemCurrent.countBuyedItem >= shopItem.maxCountItemsBuyed && shopItem.maxCountItemsBuyed != -1)
                usedNames.Add(itemCurrent.nameItem);
        }
        List<ShopItem> result = new List<ShopItem>();
        foreach (ShopItem item in customItems)
        {
            if (!usedNames.Contains(item.nameItem))
                result.Add(item);
        }
        return result;
    }
    public void DropCountItem(string nameItem)
    {
        for (int i = 0; i < itemsShopNames.Count; i++)
        {
            if (itemsShopNames[i].nameItem == nameItem)
            {
                itemsShopNames[i].countBuyedItem++;
                print(itemsShopNames[i].countBuyedItem);
            }
        }
    }
}
