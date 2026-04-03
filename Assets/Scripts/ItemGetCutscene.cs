using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGetCutscene : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string nameItem;
        public GameObject item;
    }
    [Header("Values:")]
    [SerializeField] GameObject cameraItemGet;
    [SerializeField] Item[] items;
    public static ItemGetCutscene instance;
    Player player;
    Animator anim;
    void Start()
    {
        instance = this;
        anim = GetComponentInChildren<Animator>();
        player = GetComponent<Player>();
    }    
    public void PlayItemGetCutscene(string itemName)=>StartCoroutine(cutsceneItemGet(itemName));
    IEnumerator cutsceneItemGet(string itemName)
    {
        GameManager.instance.blackLines.SetInteger("FadeState",1);
        player.gun.enabled = false;
        player.canMove = false;
        player.gun.mainGun.localRotation = Quaternion.Euler(11,0,0);
        foreach(Item item in items)
            item.item.SetActive(false);
        cameraItemGet.SetActive(true);
        anim.SetBool("GetItemStart",true);
        yield return new WaitForSeconds(1f);
        items.First(x => x.nameItem == itemName).item.SetActive(true);
        anim.SetBool("GetItem",true);
        yield return new WaitForSeconds(2f);
        cameraItemGet.SetActive(false);
        GameManager.instance.blackLines.SetInteger("FadeState",-1);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("GetItem",false);
        cameraItemGet.SetActive(false);
        player.gun.enabled = true;
        player.canMove = true;
    }
}
