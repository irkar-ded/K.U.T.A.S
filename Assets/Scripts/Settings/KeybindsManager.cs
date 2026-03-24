using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindsManager : MonoBehaviour
{
    [SerializeField] GameObject buttonKeybind;
    [SerializeField] Transform contentKeybinds;
    List<Keybinds> keybinds = new List<Keybinds>();
    public bool isRebinding = false;
    void Start() => createButtons();
    public void tryToRebind(SettingsManager.Keybinds save, string bind)
    {
        int button = getIdKeybind(save);
        int tempId = getCopyBind(bind);
        if (tempId != -1)
        {
            SettingsManager.instance.SaveKeybinds(SettingsManager.instance.settings.gameSettings.keybinds[tempId].save, SettingsManager.instance.settings.gameSettings.keybinds[button].keyBind);
            SettingsManager.instance.SaveKeybinds(SettingsManager.instance.settings.gameSettings.keybinds[button].save, bind);
        }
        else
            SettingsManager.instance.SaveKeybinds(SettingsManager.instance.settings.gameSettings.keybinds[button].save, bind);
        for (int i = 0; i < SettingsManager.instance.settings.gameSettings.keybinds.Length; i++)
            keybinds[i].RefreshInfo();
        isRebinding = false;
    }
    public void createButtons()
    {
        for (int i = 0; i < SettingsManager.instance.settings.gameSettings.keybinds.Length; i++)
        {
            Keybinds keybind = Instantiate(buttonKeybind, contentKeybinds).GetComponent<Keybinds>();
            keybind.SetButton(SettingsManager.instance.settings.gameSettings.keybinds[i].save);
            keybinds.Add(keybind);
        }
    }
    public int getIdKeybind(SettingsManager.Keybinds save)
    {
        for(int i = 0; i < SettingsManager.instance.settings.gameSettings.keybinds.Length; i++)
        {
            if (keybinds[i].currentKeybind == save)
                return i;
        }
        return -1;
    }
    public int getCopyBind(string bind)
    {
        for(int i =0;i < SettingsManager.instance.settings.gameSettings.keybinds.Length;i++)
        {
            if (bind == SettingsManager.instance.getKeybind(SettingsManager.instance.settings.gameSettings.keybinds[i].save))
                return i;
        }
        return -1;
    }
}
