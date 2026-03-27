using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Serializable]
    public class SoundSettings
    {
        public VolumePlayer[] volumesPlayer =
        {
            new VolumePlayer("Game",1),
            new VolumePlayer("Music",1),
            new VolumePlayer("SFX",1)
        };
    }
    [Serializable]
    public class VolumePlayer
    {
        public string save;
        public float volumeValue;
        public VolumePlayer(string nameSave, float volume)
        {
            this.save = nameSave;
            this.volumeValue = volume;
        }
    }
    [Serializable]
    public class keybindSave
    {
        public Keybinds save;
        public string keyBind;
        public keybindSave(Keybinds key,string bind)
        {
            this.save = key;
            this.keyBind = bind;
        }
    }
    [Serializable]
    public class VideoSettings
    {
        public int idResolution = -1;
        public int idGraphicsQuality = -1;
        public int pixelScale = 1;
        public bool vsync = false;
        public bool fullScreen = true;
        public bool seeFrameRate = false;
    }
    [Serializable]
    public class GameSettings
    {
        public keybindSave[] keybinds = 
        {
            new keybindSave(Keybinds.Fire,"<Mouse>/leftButton"),
            new keybindSave(Keybinds.Pause,"<Keyboard>/escape"),
            new keybindSave(Keybinds.Restart,"<Keyboard>/r"),
            new keybindSave(Keybinds.Use,"<Keyboard>/r")
        };
    }
    [Serializable]
    public class Settings
    {
        public SoundSettings soundSettings;
        public VideoSettings videoSettings;
        public GameSettings gameSettings;
    }
    [Header("Main:")]
    [SerializeField]
    public Settings settings;
    [HideInInspector] public UnityEvent changeKey;
    [Header("Panels Settings")]
    [SerializeField] CanvasGroup[] panels;
    [SerializeField] CanvasGroup panelSettings;
    [SerializeField] CanvasGroup panelExit;
    CanvasGroup lastPanelOpen;
    [Header("Set:")]
    [SerializeField] GameObject frameRateText;
    [SerializeField] global::VideoSettings videoSettings;
    [SerializeField] SetVolume[] volumeSettings;
    [Header("Sound")]
    [SerializeField] EventReference soundOpen;
    [SerializeField] EventReference soundClose;
    public static Controls gameInputs;
    public static SettingsManager instance;
    MovePanelAnimation movePanelAnimation;
    string pathSave;
    // Start is called before the first frame update
    void Awake()
    {
        if (gameInputs == null)
            gameInputs = new Controls();
        movePanelAnimation = GetComponentInParent<MovePanelAnimation>();
        instance = this;
        pathSave = Path.Combine(Application.persistentDataPath, "settings.json");
        if (File.Exists(pathSave))
        {
            Settings tempVolume = JsonUtility.FromJson<Settings>(File.ReadAllText(pathSave));
            if (tempVolume == null || tempVolume != null && tempVolume.gameSettings.keybinds.Length < settings.gameSettings.keybinds.Length)
                CreateSave();
            else
                LoadSave();
        }
        else
            CreateSave();
    }
    public void OpenOrCloseSettings(bool open)
    {
        if (open)
        {
            foreach(CanvasGroup panel in panels)
            {
                panel.alpha = 0;
                panel.gameObject.SetActive(false);
            }
            panels[0].alpha = 1;
            panels[0].gameObject.SetActive(true);
            movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(panelSettings,panelExit));
            RuntimeManager.PlayOneShot(soundOpen);
            lastPanelOpen = panels[0];
        }
        else
        {
            movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(panelExit,panelSettings));
            RuntimeManager.PlayOneShot(soundClose);
        }
    }
    public void OpenPanelSettings(int id)
    {
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(panels[id],lastPanelOpen));
        lastPanelOpen = panels[id];
    }
    public void CreateSave()
    {
        Save();
        LoadSave();
    }
    public void PrepareKeybinds()
    {
        for (int i = 0; i < settings.gameSettings.keybinds.Length; i++)
            RebindKey(settings.gameSettings.keybinds[i].save, settings.gameSettings.keybinds[i].keyBind);
    }
    public void PrepareNullKeybinding()
    {
        if (cheakNullKeybinds() == false)
            return;
            for (int i = 0; i < settings.gameSettings.keybinds.Length; i++)
            {
                switch (settings.gameSettings.keybinds[i].save)
                {
                    case Keybinds.Fire:
                        settings.gameSettings.keybinds[i].keyBind = gameInputs.Player.Fire.bindings[0].path;
                        break;
                    case Keybinds.Pause:
                        settings.gameSettings.keybinds[i].keyBind = gameInputs.Player.Pause.bindings[0].path;
                        break;
                    case Keybinds.Restart:
                        settings.gameSettings.keybinds[i].keyBind = gameInputs.Player.Restart.bindings[0].path;
                        break;
                    case Keybinds.Use:
                        settings.gameSettings.keybinds[i].keyBind = gameInputs.Player.Use.bindings[0].path;
                        break;
                }
            }
    }
    public InputAction getInputAction(Keybinds save)
    {
        switch (save)
        {
            case Keybinds.Fire:
                return gameInputs.Player.Fire;
            case Keybinds.Pause:
                return gameInputs.Player.Pause;
            case Keybinds.Restart:
                return gameInputs.Player.Restart;
            case Keybinds.Use:
                return gameInputs.Player.Use;
        }
        return null;
    }
    public void LoadSave()
    {
        settings = JsonUtility.FromJson<Settings>(File.ReadAllText(pathSave));
        videoSettings.PrepareSettings();
        foreach (SetVolume setVolume in volumeSettings)
            setVolume.PrepareSettings();
        PrepareNullKeybinding();
        PrepareKeybinds();
        frameRateText.SetActive(settings.videoSettings.seeFrameRate);
    }
    public bool cheakNullKeybinds()
    {
        foreach (keybindSave keybind in settings.gameSettings.keybinds)
        {
            if (string.IsNullOrEmpty(keybind.keyBind))
                return true;
        }
        return false;
    }
    public string getKeybind(Keybinds save)
    {
        foreach (keybindSave keybind in settings.gameSettings.keybinds)
        {
            if (keybind.save == save)
                return keybind.keyBind;
        }
        return "";
    }
    public float getVolume(string save)
    {
        foreach (VolumePlayer volumePlayer in settings.soundSettings.volumesPlayer)
        {
            if (volumePlayer.save == save)
                return volumePlayer.volumeValue;
        }
        return -1;
    }
    public void SaveKeybinds(Keybinds keybind, string rebindKey)
    {
        for (int i = 0; i < settings.gameSettings.keybinds.Length; i++)
        {
            if (settings.gameSettings.keybinds[i].save == keybind)
            {
                settings.gameSettings.keybinds[i].keyBind = rebindKey;
                RebindKey(keybind, rebindKey);
            }
        }
        Save();
        changeKey.Invoke();
    }
    public void RebindKey(Keybinds save, string path)
    {
        switch (save)
        {
            case Keybinds.Fire:
                gameInputs.Player.Fire.ApplyBindingOverride(0, path);
                break;
            case Keybinds.Pause:
                gameInputs.Player.Pause.ApplyBindingOverride( 0, path);
                break;
            case Keybinds.Restart:
                gameInputs.Player.Restart.ApplyBindingOverride(0, path);
                break;
            case Keybinds.Use:
                gameInputs.Player.Use.ApplyBindingOverride(0, path);
                break;
        }
    }
    public enum Keybinds
    {
        Fire,
        Pause,
        Restart,
        Use
    }
    public void saveVsync(bool onVsync)
    {
        settings.videoSettings.vsync = onVsync;
        Save();
    }
    public void saveFullscreen(bool fullscreen)
    {
        settings.videoSettings.fullScreen = fullscreen;
        Save();
    }
    public void saveSeeFrameRate(bool canSee)
    {
        settings.videoSettings.seeFrameRate = canSee;
        Save();
        frameRateText.SetActive(settings.videoSettings.seeFrameRate);
    }
    public void saveResolution(int id)
    {
        settings.videoSettings.idResolution = id;
        Save();
    }
    public void saveQuality(int id)
    {
        settings.videoSettings.idGraphicsQuality = id;
        Save();
    }
    void Save() => File.WriteAllText(pathSave, JsonUtility.ToJson(settings));
    public void SaveVolume(string save, float volume)
    {
        for (int i = 0; i < settings.soundSettings.volumesPlayer.Length; i++)
        {
            if (settings.soundSettings.volumesPlayer[i].save == save)
            {
                settings.soundSettings.volumesPlayer[i].volumeValue = volume;
                Save();
                break;
            }
        }
    }
    public void savePixelScale(int value)
    {
        settings.videoSettings.pixelScale = value;
        Save();
    }
}
