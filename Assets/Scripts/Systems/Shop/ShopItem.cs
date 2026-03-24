using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item",menuName ="Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    public Sprite iconItem;
    public string nameItem;
    public string description;
    public int costItem;
    public int maxCountItemsBuyed = 10;
    public BuffItem buff;
}
