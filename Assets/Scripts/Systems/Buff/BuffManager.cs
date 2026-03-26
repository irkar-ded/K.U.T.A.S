using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    [System.Serializable]
    public class PassiveBuff
    {
        public float fireRate;
        public float maxHealth;
        public float bonusSpeed;
        public float bonusForceBullet;
        public float damage;
        public int bonusCoin;
        public float lessTimeFade;
        public int toxicBullet;
        public int xRayBullet;
        public int bounceBullet;
        public bool isExplosionAfterDeath;
    }
    public class UIStackItemHolder
    {
        public BuffItem buff;
        public int count;
        public UIStackItemHolder(BuffItem buff)
        {
            this.buff = buff;
            count = 1;
        }
    }
    [SerializeField] GameObject textYourBuffs;
    [SerializeField] Transform endBuffsContent;
    [SerializeField] GameObject buffIcon;
    [SerializeField] Transform contentIcons;
    List<GameObject> buffIcons = new List<GameObject>();
    public PassiveBuff passiveBuff;
    public List<BuffItem> itemsBuff;
    public static UnityEvent<string> onBuff;
    public static BuffManager instance;

    void Start()=>instance = this;
    public void setPassiveBuff()
    {
        PassiveBuff tempBuff = new PassiveBuff();
        foreach(BuffItem item in itemsBuff)
        {
            if(item.typeBuffOn == BuffItem.TYPE_BUFF_ON.Passive)
            {
                tempBuff.maxHealth += item.passiveBuff.maxHealth;
                tempBuff.fireRate += item.passiveBuff.fireRate;
                tempBuff.damage += item.passiveBuff.damage;
                tempBuff.bonusCoin += item.passiveBuff.bonusCoin;
                tempBuff.lessTimeFade += item.passiveBuff.lessTimeFade;
                tempBuff.bonusSpeed += item.passiveBuff.bonusSpeed;
                tempBuff.toxicBullet += item.passiveBuff.toxicBullet;
                tempBuff.xRayBullet += item.passiveBuff.xRayBullet;
                tempBuff.bounceBullet += item.passiveBuff.bounceBullet;
                tempBuff.bonusForceBullet += item.passiveBuff.bonusForceBullet;
                tempBuff.isExplosionAfterDeath = item.passiveBuff.isExplosionAfterDeath;
            }
        }
        passiveBuff = tempBuff;
    }
    public void cheakUIContent()
    {
        foreach (GameObject t in buffIcons)
            Destroy(t);
        buffIcons.Clear();
        List<UIStackItemHolder> UIStackItemHolders = new List<UIStackItemHolder>();
        for(int j = 0;j < itemsBuff.Count; j++)
        {
            if(UIStackItemHolders.Exists(x=>x.buff.name == itemsBuff[j].name))
                UIStackItemHolders.Find(x=>x.buff.name == itemsBuff[j].name).count++;
            else
                UIStackItemHolders.Add(new UIStackItemHolder(itemsBuff[j]));
        }
        for(int i = 0;i < UIStackItemHolders.Count; i++)
        {
            if (UIStackItemHolders[i].buff.icon != null)
            {
                Image tempImg = Instantiate(buffIcon, contentIcons).GetComponent<Image>();
                TextMeshProUGUI textStackBuff = tempImg.GetComponentInChildren<TextMeshProUGUI>();
                if(UIStackItemHolders[i].count > 1)
                    textStackBuff.text = UIStackItemHolders[i].count.ToString();
                else
                    textStackBuff.gameObject.SetActive(false);
                tempImg.sprite = UIStackItemHolders[i].buff.icon;
                buffIcons.Add(tempImg.gameObject);
            }
        }
    }
    public void cheakUIEndContent()
    {
        if (itemsBuff.Count <= 0)
            return;
        textYourBuffs.SetActive(true);
        List<UIStackItemHolder> UIStackItemHolders = new List<UIStackItemHolder>();
        for(int j = 0;j < itemsBuff.Count; j++)
        {
            if(UIStackItemHolders.Exists(x=>x.buff == itemsBuff[j]))
                UIStackItemHolders.Find(x=>x.buff == itemsBuff[j]).count++;
            else
                UIStackItemHolders.Add(new UIStackItemHolder(itemsBuff[j]));
        }
        for (int i = 0; i < UIStackItemHolders.Count; i++)
        {
            if (UIStackItemHolders[i].buff.icon != null)
            {
                Image tempImg = Instantiate(buffIcon, endBuffsContent).GetComponent<Image>();
                TextMeshProUGUI textStackBuff = tempImg.GetComponentInChildren<TextMeshProUGUI>();
                if(UIStackItemHolders[i].count > 1)
                    textStackBuff.text = UIStackItemHolders[i].count.ToString();
                else
                    textStackBuff.gameObject.SetActive(false);
                tempImg.sprite = UIStackItemHolders[i].buff.icon;
            }
        }
    }
    public bool getStateBuff(string name)
    {
       foreach(BuffItem buff in itemsBuff)
       {
            if (buff.nameBuff == name)
                return true;
       }
       return false;
    }
    public float getCurrentTimeBuff(string name)
    {
        foreach (BuffItem buff in itemsBuff)
        {
            if (buff.nameBuff == name)
                return buff.timerBuff;
        }
        return -1;
    }
    public void addBuff(BuffItem buff)
    {
        BuffItem currentBuff = Instantiate(buff.gameObject, transform).GetComponent<BuffItem>();
        itemsBuff.Add(currentBuff);
        setPassiveBuff();
        cheakUIContent();
    }
    public void removeBuff(BuffItem buff)
    {
        itemsBuff.Remove(buff);
        Destroy(buff.gameObject);
        setPassiveBuff();
        cheakUIContent();
    }
    public void SendBuff(string name) => onBuff.Invoke(name);
}
