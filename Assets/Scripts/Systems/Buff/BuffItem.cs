using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : MonoBehaviour
{
    public enum TYPE_BUFF_ON {
        StartAndThenDestroy,
        Recycle,
        Passive
    }
    public string nameBuff;
    public Sprite icon;
    public BuffManager.PassiveBuff passiveBuff;
    public TYPE_BUFF_ON typeBuffOn;
    public float timeBuff = 1;
    [HideInInspector]public float timerBuff;
    private void Start()=>timerBuff = timeBuff;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.gameIsStarted == false) return;
        if(typeBuffOn != TYPE_BUFF_ON.Passive)
        {
            if (timerBuff > 0)
                timerBuff -= Time.deltaTime;
            if (timerBuff <= 0)
                onEndCycle();
        }
    }
    public void onEndCycle()
    {
        switch (typeBuffOn)
        {
            case TYPE_BUFF_ON.StartAndThenDestroy:
                BuffManager.instance.removeBuff(this);
                break;
            case TYPE_BUFF_ON.Recycle:
                timerBuff = timeBuff;
                BuffManager.instance.SendBuff(nameBuff);
                break;
        }
    }
}
