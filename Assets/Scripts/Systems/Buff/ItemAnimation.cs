using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
    [SerializeField] Vector3 rot = Vector3.up * 100;
    float startYPos;
    void Start()=>startYPos = transform.position.y;
    void Update()
    {
       transform.Rotate(rot * Time.deltaTime); 
       transform.position = new Vector3(transform.position.x,startYPos - Mathf.Sin(Time.time) * 0.5f,transform.position.z);
    }
}
