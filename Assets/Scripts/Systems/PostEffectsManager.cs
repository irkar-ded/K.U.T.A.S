using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostEffectsManager : MonoBehaviour
{
    PostProcessVolume postProcessVolume;
    ColorGrading colorGrading;
    bool isDeath;
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
        colorGrading.saturation.value = timerBlackAndWhite;
    }
    public void SetBackAndWhite(bool deathState) => isDeath = deathState;
}
