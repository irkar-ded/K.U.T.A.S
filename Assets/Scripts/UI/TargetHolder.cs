using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetHolder : MonoBehaviour
{
    public class Pointer
    {
        public RectTransform holder;
        public Transform target;
        public Pointer(RectTransform holder,Transform target)
        {
            this.holder = holder;
            this.target = target;
        }
    }
    [SerializeField] GameObject holderPrefab;

    public static TargetHolder instance;
    [SerializeField]float borderSize = 50;
    List<Pointer> pointers = new List<Pointer>();
    void Awake()=>instance = this;
    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i < pointers.Count; i++)
        {
            if(pointers[i].target == null)
            {
                DestroyPoint(pointers[i].target);
                continue;
            }
            Vector2 targetPosition = Camera.main.WorldToScreenPoint(pointers[i].target.position);
            bool isOffScreen = targetPosition.x <= borderSize || targetPosition.x >= Screen.width - borderSize || targetPosition.y <= borderSize || targetPosition.y >= Screen.height - borderSize;
            if(!isOffScreen || GameManager.instance.gameIsStarted == false || GameManager.instance.endGameState)
            {
                pointers[i].holder.gameObject.SetActive(false);
                continue;
            }
            else if(pointers[i].holder.gameObject.activeSelf == false)
                pointers[i].holder.gameObject.SetActive(true);
            Vector3 dir =  (targetPosition - (Vector2)pointers[i].holder.position).normalized;
            float angle = Mathf.Atan2(dir.y,dir.x) * (180/Mathf.PI);
            pointers[i].holder.eulerAngles = new Vector3(0,0,angle - 90);
            targetPosition.x = Mathf.Clamp(targetPosition.x,borderSize,Screen.width - borderSize);
            targetPosition.y = Mathf.Clamp(targetPosition.y,borderSize,Screen.height - borderSize);
            pointers[i].holder.position = targetPosition;
        }
    }
    public void CreatePoint(Transform target)=>pointers.Add(new Pointer(Instantiate(holderPrefab,transform).GetComponent<RectTransform>(),target));
    public void DestroyPoint(Transform target)
    {
        Pointer pointer = pointers.Find(x => x.target == target);
        pointers.Remove(pointer);
        Destroy(pointer.holder.gameObject);
    }
}
