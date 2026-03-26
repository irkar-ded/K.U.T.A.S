using EZ_Pooling;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupDamage : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] Color colorDefualt;
    [SerializeField] Color colorCritical;
    [SerializeField] Vector2 force;
    [SerializeField] float lifetime = 1f;
    float timerForce = 0f;
    float startY;
    float currentForceX;
    float timer = 0f;
    TextMeshPro _text;
    MeshRenderer mesh;
    //Rigidbody rb;
    // Update is called once per frame
    void Update()
    {
        if(mesh == null || _text == null)
            return;
        timer += Time.unscaledDeltaTime;
        float t = timer / lifetime;
        mesh.material.SetColor("_Color", Color.Lerp(mesh.material.GetColor("_Color"), Color.clear,t / 5));
        mesh.material.SetColor("_ColorOutline", Color.Lerp(mesh.material.GetColor("_ColorOutline"), Color.clear, t / 5));
        transform.position = new Vector3(transform.position.x, transform.position.y, startY + Mathf.Sin(t) * force.y) + transform.right * Time.deltaTime * currentForceX;
        if (timer >= 1f)
           EZ_PoolManager.Despawn(transform);
    }
    public void SetText(float damage,bool isCritical)
    {
        if (_text == null)
            _text = GetComponentInChildren<TextMeshPro>();
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();
        /*if (rb == null)
            rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;*/
        timer = 0f;
        //text.color = Color.white;
        //rb.AddForce(transform.right * ((Random.value > 0.5f ? -1 : 1) * force.x) + transform.up * force.y, ForceMode.Impulse);
        _text.text = $"-{damage}";
        startY = transform.position.y;
        currentForceX = force.x * (Random.value > 0.5f ? -1 : 1);
        mesh.material.SetColor("_ColorOutline", isCritical ? colorCritical : colorDefualt);
    }
    public void SetText(string text, bool isCritical)
    {
        if (_text == null)
            _text = GetComponentInChildren<TextMeshPro>();
        if (mesh == null)
            mesh = GetComponentInChildren<MeshRenderer>();
        /* if (rb == null)
             rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;*/
        timer = 0f;
        //text.color = Color.white;
        //rb.AddForce(transform.right * ((Random.value > 0.5f ? -1 : 1) * force.x) + transform.up * force.y, ForceMode.Impulse);
        _text.text = text;
        //startX = transform.position.x;
        startY = transform.position.z;
        currentForceX = force.x * (Random.value > 0.5f ? -1 : 1);
        mesh.material.SetColor("_ColorOutline", isCritical ? colorCritical : colorDefualt);
    }
}
