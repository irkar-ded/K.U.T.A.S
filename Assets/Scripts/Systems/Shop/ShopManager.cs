using System.Collections.Generic;
using System.Linq;
using EZ_Pooling;
using FMODUnity;
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
    [SerializeField] RefreshItem itemRefresh;
    [SerializeField] ShopItemHolder itemShopHolder;
    [SerializeField] Transform contentItems;
    [SerializeField] ShopItem[] itemsShop;
    [SerializeField] ShopItem[] customItems;
    List<ShopItemTemp> itemsShopNames = new List<ShopItemTemp>();
    List<ShopItemHolder> currentItems = new List<ShopItemHolder>();
    public static ShopManager instance;
    [SerializeField] public UnityEvent onBuy;
    [Header("Sound")]
    [SerializeField] EventReference soundOpen;
    [SerializeField] EventReference soundExit;
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
        RuntimeManager.PlayOneShot(soundOpen);
        createItems();
    }
    public void createItems()
    {
        if (customItems.Length > 0)
        {
            for(int j = 0;j < customItems.Length; j++)
            {
                ShopItemHolder randomItem = Instantiate(itemShopHolder, contentItems);
                randomItem.SetItemHolder(NewCanSpawnItemsCustom(customItems.ToList())[Mathf.Clamp(j, 0, NewCanSpawnItemsCustom(customItems.ToList()).Count - 1)]);
                currentItems.Add(randomItem);
            }
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                List<ShopItem> tempItems = NewCanSpawnItems();
                ShopItemHolder randomItem = Instantiate(itemShopHolder, contentItems);
                randomItem.SetItemHolder(tempItems[Random.Range(0, tempItems.Count)]);
                currentItems.Add(randomItem);
            }
        }
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
        RuntimeManager.PlayOneShot(soundExit);
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
    public List<ShopItem> NewCanSpawnItems()
    {
        List<ShopItem> tempItems = new List<ShopItem>(itemsShop);
        foreach(ShopItemTemp item in itemsShopNames)
        {
            if(currentItems.Exists(x => item.nameItem == x.item.nameItem))
            {
                tempItems.Remove(tempItems.Find(x => x.nameItem == item.nameItem));
                continue;
            }
            ShopItem tempItem = tempItems.Find(x => x.nameItem == item.nameItem);
            if(item.countBuyedItem >= tempItem.maxCountItemsBuyed && tempItem.maxCountItemsBuyed != -1)
                tempItems.Remove(tempItem);
        }
        return tempItems;
    }
    public List<ShopItem> NewCanSpawnItemsCustom(List<ShopItem> customItems)
    {
        List<ShopItem> tempItems = new List<ShopItem>(customItems);
        foreach(ShopItem item in customItems)
        {
            if(!currentItems.Exists(x => item.nameItem == x.item.nameItem))
                continue;
            ShopItem tempItem = tempItems.Find(x => x.nameItem == item.nameItem);
            if(itemsShopNames.Find(x => x.nameItem == item.nameItem).countBuyedItem >= tempItem.maxCountItemsBuyed && tempItem.maxCountItemsBuyed != -1)
                tempItems.Remove(tempItem);
        }
        return tempItems;
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
