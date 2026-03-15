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
    [SerializeField] GameObject itemRefresh;
    [SerializeField] Transform[] itemsHolder;
    [SerializeField] Transform refreshItemHolder;
    [SerializeField] ShopItem[] itemsShop;
    [SerializeField] ShopItem[] customItems;
    List<ShopItemTemp> itemsShopNames = new List<ShopItemTemp>();
    List<ShopItem> currentItems = new List<ShopItem>();
    public static ShopManager instance;
    [SerializeField] public UnityEvent onBuy;
    RefreshItem refreshItem;
    // Start is called before the first frame update
    void Awake()=>instance = this;
    public void createItems()
    {
        //bool isOutStock = false;
        if (customItems.Length > 0)
        {
           /* if (canSpawnItemsCustom(customItems.ToList()).Count <= 0)
                isOutStock = true;
            else
            {*/
                for(int j = 0;j < itemsHolder.Length; j++)
                {
                    ShopItem randomItem = Instantiate(canSpawnItemsCustom(customItems.ToList())[Mathf.Clamp(j, 0, canSpawnItemsCustom(customItems.ToList()).Count - 1)], itemsHolder[j]);
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
                for(int j = 0;j < itemsHolder.Length; j++)
                {
                    List<ShopItem> tempItems = canSpawnItems();
                    ShopItem randomItem = Instantiate(tempItems[Random.Range(0, tempItems.Count)], itemsHolder[j]);
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
        refreshItem = Instantiate(itemRefresh, refreshItemHolder).GetComponent<RefreshItem>();
        refreshItem.SetItem();
    }
    public void Refresh()
    {
        destroyItems();
        createItems();
    }
    private void OnDisable() => destroyItems();
    public void destroyItems()
    {
        foreach (ShopItem shopButton in currentItems)
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
        foreach (ShopItem itemCurrent in currentItems)
            usedNames.Add(itemCurrent.name);
        foreach (ShopItemTemp itemCurrent in itemsShopNames)
        {
            if (itemCurrent.countBuyedItem >= itemsShop.First(x=> x.nameItem == itemCurrent.nameItem).maxCountItemsBuyed && itemsShop.First(x=> x.nameItem == itemCurrent.nameItem).maxCountItemsBuyed != -1)
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
            if (itemCurrent.countBuyedItem >= itemsShop.First(x=> x.nameItem == itemCurrent.nameItem).maxCountItemsBuyed && itemsShop.First(x=> x.nameItem == itemCurrent.nameItem).maxCountItemsBuyed != -1)
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
