using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] Vector3 endPos;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * 3);
        if(Vector3.Distance(transform.position,endPos) <= 0.1f)
            Destroy(this);
    }
}
