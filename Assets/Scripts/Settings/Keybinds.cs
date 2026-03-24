using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Keybinds : MonoBehaviour
{
    [System.Serializable]
    public class customKeyVisible
    {
        public string key;
        public string text;
        public customKeyVisible(string key, string text)
        {
            this.key = key;
            this.text = text;
        }
    }
    [SerializeField] customKeyVisible[] customKeyVisibles;
    public SettingsManager.Keybinds currentKeybind;
    TextMeshProUGUI keybindText;
    KeybindsManager keybindsManager;
    InputAction dummyAction;
    private void Awake() => dummyAction = SettingsManager.gameInputs.Player.NullKey;
    public void SetButton(SettingsManager.Keybinds keybind)
    {
        keybindsManager = GetComponentInParent<KeybindsManager>();
        keybindText = GetComponentInChildren<TextMeshProUGUI>();
        currentKeybind = keybind;
        RefreshInfo();
    }
    public void RefreshInfo()
    {
        keybindText.text = currentKeybind + ":<color=yellow>" + InputControlPath.ToHumanReadableString(SettingsManager.instance.getKeybind(currentKeybind), InputControlPath.HumanReadableStringOptions.OmitDevice) + "</color>";
        foreach (customKeyVisible keyVisible in  customKeyVisibles)
        {
            if(keyVisible.key == SettingsManager.instance.getKeybind(currentKeybind))
                keybindText.text = currentKeybind + ":<color=yellow>" + keyVisible.text + "</color>";
        }
    }
    public void OnChangeKey()
    {
        if(keybindsManager.isRebinding)
            return;
        keybindsManager.isRebinding = true;
        keybindText.SetText("AWAITING INPUT");
        dummyAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Gamepad>")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                var path = operation.selectedControl.path;
                keybindsManager.tryToRebind(currentKeybind, path);
                keybindsManager.isRebinding = false;
                operation.Dispose();
            })
            .Start();
    }
}
