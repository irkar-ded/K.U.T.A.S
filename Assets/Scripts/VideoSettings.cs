using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class VideoSettings : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle showFPSToggle;
    [SerializeField] Toggle vsyncToggle;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown qualityDropdown;
    Resolution[] resolutions;
    [Header("Pixel Scale:")]
    //[SerializeField] TextMeshProUGUI textPixelScaleValue;
    [SerializeField] Slider sliderPixelScale;
    public void PrepareSettings()
    {
        PrepareResolution();
        PrepareQuality();
        LoadSettings();
    }
    public void LoadPixelScale()
    {
        sliderPixelScale.value = SettingsManager.instance.settings.videoSettings.pixelScale;
        //textPixelScaleValue.text = sliderPixelScale.value.ToString();
    }
    public void SavePixelScale()
    {
        SettingsManager.instance.savePixelScale((int)sliderPixelScale.value);
        //textPixelScaleValue.text = sliderPixelScale.value.ToString();
        //PlayerPrefs.SetFloat(saveIndex, newVolume);
        //audioLayer.setVolume(newVolume);
    }
    public void PrepareResolution()
    {
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }
    public void PrepareQuality()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            string option = QualitySettings.names[i];
            options.Add(option);
        }
        qualityDropdown.AddOptions(options);
        qualityDropdown.RefreshShownValue();
    }
    public void SetVsync(bool onVsync)
    {
        SettingsManager.instance.saveVsync(onVsync);
        QualitySettings.vSyncCount = onVsync ? -1 : 1;
    }
    public void SetShowFPS(bool onShowFPS)=>SettingsManager.instance.saveSeeFrameRate(onShowFPS);
    public void SetFullscreen(bool isFullscreen)
    {
        SettingsManager.instance.saveFullscreen(isFullscreen);
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        SettingsManager.instance.saveResolution(resolutionIndex);
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        SettingsManager.instance.saveQuality(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void LoadResolution()
    {
        if (SettingsManager.instance.settings.videoSettings.idResolution != -1)
        {
            resolutionDropdown.value = SettingsManager.instance.settings.videoSettings.idResolution;
            SetResolution(resolutionDropdown.value);
        }
        else
        {
            SettingsManager.instance.settings.videoSettings.idResolution = resolutionDropdown.options.Count;
            resolutionDropdown.value = resolutionDropdown.options.Count;
        }
    }
    public void LoadQuality()
    {
        if (SettingsManager.instance.settings.videoSettings.idGraphicsQuality != -1)
        {
            qualityDropdown.value = SettingsManager.instance.settings.videoSettings.idGraphicsQuality;
            SetQuality(qualityDropdown.value);
        }
        else
        {
            SettingsManager.instance.settings.videoSettings.idGraphicsQuality = QualitySettings.GetQualityLevel();
            qualityDropdown.value = qualityDropdown.options.Count;
        }
    }
    public void LoadFullscreen()
    {
        fullscreenToggle.isOn = SettingsManager.instance.settings.videoSettings.fullScreen;
        SetFullscreen(fullscreenToggle.isOn);
    }
    public void LoadVsync()
    {
        vsyncToggle.isOn = SettingsManager.instance.settings.videoSettings.vsync;
        SetVsync(vsyncToggle.isOn);
    }    
    public void LoadShowFPS()
    {
        showFPSToggle.isOn = SettingsManager.instance.settings.videoSettings.seeFrameRate;
        SetShowFPS(showFPSToggle.isOn);
    }
    public void LoadSettings()
    {
        LoadResolution();
        LoadQuality();
        LoadFullscreen();
        LoadVsync();
        LoadShowFPS();
        LoadPixelScale();
    }
}
