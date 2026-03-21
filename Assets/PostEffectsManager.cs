using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectsManager : MonoBehaviour
{
    PostProcessVolume postProcessVolume;
    ColorGrading colorGrading;
    bool isDeath;
    //float timerContrast;
    float timerBlackAndWhite;
    public static PostEffectsManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        postProcessVolume = GetComponent<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        timerBlackAndWhite = colorGrading.saturation.value;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (timerContrast > 0)
            timerContrast -= Time.deltaTime * 150;*/
        if (isDeath)
        {
            if (timerBlackAndWhite > -100)
                timerBlackAndWhite -= Time.unscaledDeltaTime * 100;
        }
        else
        {
            if (timerBlackAndWhite < 0)
                timerBlackAndWhite += Time.unscaledDeltaTime * 100;
        }
        //colorGrading.contrast.value = timerContrast;
        colorGrading.saturation.value = timerBlackAndWhite;
    }
    //public void SetContrast() => timerContrast = 100;
    public void SetBackAndWhite(bool deathState) => isDeath = deathState;
}
