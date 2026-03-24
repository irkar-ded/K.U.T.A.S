using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start() => text = GetComponent<TextMeshProUGUI>();

    // Update is called once per frame
    void Update()=>text.text = $"FPS:{(int)(1.0f / Time.unscaledDeltaTime)}";
}
