using System;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyReplacer : MonoBehaviour
{
    public string text;
    TextMeshProUGUI textComponent;
    string startText;
    public static string SetKeybindInText(string text)
    {
        string finalText = text;
        while(finalText.IndexOf('#') != -1)
        {
            int startBindId = finalText.IndexOf('#');
            int endBindId = finalText.IndexOf('#',startBindId+1);
            string bindName = finalText.Substring(startBindId + 1,endBindId-startBindId-1);
            finalText = finalText.Replace($"#{bindName}#",getTextKey(bindName));
        }
        return finalText;
    }
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        SetText();
    }
    public string getTextFinal() => SetKeybindInText(startText);
    public void SetText(string text)
    {
        if(textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        startText = text;
        textComponent.text = SetKeybindInText(startText);
    }
    public void SetText()
    {
        if(string.IsNullOrEmpty(text))
            return;
        if(textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        startText = text;
        textComponent.text = SetKeybindInText(startText);
    }
    public static string getTextKey(string id)
    {
        string value = "";
        switch (SettingsManager.instance.getKeybind((SettingsManager.Keybinds)Enum.Parse(typeof(SettingsManager.Keybinds),id)))
        {
            case "<Keyboard>/leftCtrl":
                value = "Left Ctrl";
                break;
            case "<Keyboard>/rightCtrl":
                value = "Right Ctrl";
                break;
            case "<Mouse>/leftButton":
                value = "LMB";
                break;
            case "<Mouse>/rightButton":
                value = "RMB";
                break;
            default:
                value = InputControlPath.ToHumanReadableString(SettingsManager.instance.getKeybind((SettingsManager.Keybinds)Enum.Parse(typeof(SettingsManager.Keybinds), id)), InputControlPath.HumanReadableStringOptions.OmitDevice);
                break;
        }
        return value;
    }
}