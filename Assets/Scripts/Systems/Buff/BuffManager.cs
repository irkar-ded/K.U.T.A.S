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
        public float damage;
        public int bonusCoin;
        public float lessTimeFade;
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

    void Start()
    {
        instance = this;
    }
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
            }
        }
        passiveBuff = tempBuff;
    }
    public void cheakUIContent()
    {
        foreach (GameObject t in buffIcons)
            Destroy(t);
        buffIcons.Clear();
        for(int i = 0;i < itemsBuff.Count; i++)
        {
            if (itemsBuff[i].icon != null)
            {
                Image tempImg = Instantiate(buffIcon, contentIcons).GetComponent<Image>();
                tempImg.sprite = itemsBuff[i].icon;
                buffIcons.Add(tempImg.gameObject);
            }
        }
    }
    public void cheakUIEndContent()
    {
        if (itemsBuff.Count <= 0)
            return;
        textYourBuffs.SetActive(true);
        for (int i = 0; i < itemsBuff.Count; i++)
        {
            if (itemsBuff[i].icon != null)
            {
                Image tempImg = Instantiate(buffIcon, endBuffsContent).GetComponent<Image>();
                tempImg.sprite = itemsBuff[i].icon;
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
