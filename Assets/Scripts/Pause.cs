//using FMOD.Studio;
//using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] bool debug;
    //[SerializeField] EventReference effectPause;
    public static Pause instance;
    //EventInstance effectInstance;
    Controls gameInputs;
    InputAction pauseKey;
    public static bool canPause = true;
    //Window window;
    public static bool isPaused;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        isPaused = false;
        //window.onOpenWindow.AddListener(OnOpenPause);
        //window.onCloseWindow.AddListener(OnClosePause);
        //effectInstance = RuntimeManager.CreateInstance(effectPause);
        if (SettingsManager.instance != null)
            gameInputs = SettingsManager.gameInputs;
        else
            gameInputs = new Controls();
        pauseKey = gameInputs.Player.Pause;
    }
    private void OnDisable()
    {
        //effectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isPaused = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (debug == true)
            return;
        if (pauseKey.WasPressedThisFrame() && isPaused == false && canPause == true)
            OnOpenPause();
    }
    public void OnClosePause()
    {
        //effectInstance.start();
        Time.timeScale = 1;
        isPaused = false;
    }
    public void OnOpenPause()
    {
        isPaused = true;
        //effectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Time.timeScale = 0;
    }
    void OnApplicationPause(bool pause)
    {
        if (debug == true)
            return;
        if (pause && isPaused == false && canPause)
            OnOpenPause();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (debug == true)
            return;
        if (!hasFocus && isPaused == false && canPause)
            OnOpenPause();
    }
    private void OnEnable() => pauseKey.Enable();
    public void OnExit()
    {
        canPause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LoadingScreen.LoadScene("Menu");
    }
}
