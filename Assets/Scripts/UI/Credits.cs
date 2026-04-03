using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] Vector3 endPos;
    float timer;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()=>startPos = transform.position;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * 0.035f;
        transform.position = Vector3.Lerp(startPos,endPos,timer);
    }
}
