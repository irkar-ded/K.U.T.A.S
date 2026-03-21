using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationSkin : MonoBehaviour
{
    [System.Serializable]
    public class Skin
    {
        public string nameSkin;
        public Sprite[] frames;
    }
    [Header("Values:")]
    [SerializeField] Skin[] skins;
    [SerializeField] Image image;
    [SerializeField] int currentSkin;
    int currentFrame;
    public void SetFrame(int frame)
    {
        currentFrame = Mathf.Clamp(frame,0,skins[currentSkin].frames.Length - 1);
        image.sprite = skins[currentSkin].frames[currentFrame];
    }
    public void SetSkin(string name)=>currentSkin = Array.FindIndex(skins,x=>x.nameSkin == name);
}
