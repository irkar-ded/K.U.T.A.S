using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//using FMODUnity;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    private AsyncOperation loadingOperation;
    //[SerializeField] EventReference openSoundLoading;
    //[SerializeField] EventReference closeSoundLoading;
    [HideInInspector] public bool isLoading;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI text2;
    [SerializeField] Slider slider;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    /*private void Start()
    {
        FadeOut();
    }*/
    public static void LoadScene(string scene)
    {
        if (instance != null)
            instance.LoadSceneAnimated(scene);
        else
        {
            SceneManager.LoadScene(scene);
            Time.timeScale = 1;
            Pause.canPause = true;
        }
    }
    void LoadSceneAnimated(string sceneName)
    {
        if (!isLoading)
            StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        //MusicManager.instance.StopAllMusic();
        FadeIn();
        text.text = "0%";
        text2.text = text.text;
        slider.value = 0;
        isLoading = true;
        yield return new WaitForSecondsRealtime(0.3f);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadingOperation.isDone)
        {
            text.text = $"{Mathf.RoundToInt(loadingOperation.progress * 100)}%";
            text2.text = text.text;
            slider.value = loadingOperation.progress;
            yield return null;
        }
        text.text = "100%";
        text2.text = text.text;
        slider.value = 1;
        yield return new WaitForSecondsRealtime(0.3f);
        FadeOut();
        isLoading = false;
        Time.timeScale = 1;
        Pause.canPause = true;
    }
    public void FadeIn()
    {
        anim.SetBool("IsOpen", true);
        //RuntimeManager.PlayOneShot(openSoundLoading);
    }

    public void FadeOut()
    {
        anim.SetBool("IsOpen", false);
        //RuntimeManager.PlayOneShot(closeSoundLoading);
    }
}
