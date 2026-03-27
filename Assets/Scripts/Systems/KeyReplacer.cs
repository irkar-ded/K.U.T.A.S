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
    public static string ReplaceKeysInText(string inputText)
    {
        string pattern = "#(.*?)#";
        string result = Regex.Replace(inputText, pattern, match =>
        {
            string key = match.Groups[1].Value;
            return getTextKey(key);
        });

        return result;
    }
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        SetText();
    }
    public string getTextFinal() => ReplaceKeysInText(startText);
    public void SetText(string text)
    {
        if(textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        startText = text;
        textComponent.text = ReplaceKeysInText(startText);
    }
    public void SetText()
    {
        if(string.IsNullOrEmpty(text))
            return;
        if(textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
        startText = text;
        textComponent.text = ReplaceKeysInText(startText);
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