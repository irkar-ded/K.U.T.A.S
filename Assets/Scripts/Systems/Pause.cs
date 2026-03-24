using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
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
    [SerializeField] CanvasGroup panelUI;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Button settingsButtonExit;
    [SerializeField] bool debug;
    [Header("Sound")]
    [SerializeField] EventReference soundOpen;
    [SerializeField] EventReference soundClose;
    [SerializeField] EventReference effectPause;
    public static Pause instance;
    EventInstance effectInstance;
    Controls gameInputs;
    InputAction restartKey;
    InputAction pauseKey;
    public static bool canPause = true;
    public static bool isPaused;
    MovePanelAnimation movePanelAnimation;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        isPaused = false;
        movePanelAnimation = GetComponent<MovePanelAnimation>();
        effectInstance = RuntimeManager.CreateInstance(effectPause);
        if (SettingsManager.instance != null)
            gameInputs = SettingsManager.gameInputs;
        else
            gameInputs = new Controls();
        pauseKey = gameInputs.Player.Pause;
        restartKey = gameInputs.Player.Restart;
    }
    private void OnDisable()
    {
        effectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        pauseKey.Disable();
        restartKey.Disable();
        isPaused = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (debug == true)
            return;
        if(canPause == true)
        {
            if (pauseKey.WasPressedThisFrame() && isPaused == false)
                OnOpenPause();
            else if (pauseKey.WasPressedThisFrame() && isPaused == true)
            {
                if(settingsPanel.activeSelf)
                    settingsButtonExit.onClick.Invoke();
                else
                    OnClosePause();
            }
        }
        if(restartKey.WasPerformedThisFrame())
            LoadingScreen.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClosePause()
    {
        effectInstance.start();
        RuntimeManager.PlayOneShot(soundClose);
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(null,panelUI));
        Time.timeScale = 1;
        isPaused = false;
    }
    public void OnOpenPause()
    {
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(panelUI,null));
        isPaused = true;
        effectInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.PlayOneShot(soundOpen);
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
    private void OnEnable()
    {
        pauseKey.Enable();
        restartKey.Enable();
    }
    public void OnExit()
    {
        canPause = false;
        LoadingScreen.LoadScene("Menu");
    }
}
