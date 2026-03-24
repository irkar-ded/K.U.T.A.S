using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using FMODUnity;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    private AsyncOperation loadingOperation;
    [SerializeField] EventReference openSoundLoading;
    [SerializeField] EventReference closeSoundLoading;
    [HideInInspector] public bool isLoading;
    [SerializeField] GameObject panelLoading;
    Coroutine fadeScreenCoroutine;
    CanvasGroup canvasGroup;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        canvasGroup = GetComponent<CanvasGroup>();
        Application.targetFrameRate = -1;
        FadeOut();
    }
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
        isLoading = true;
        Time.timeScale = 0;
        Pause.canPause = false;
        panelLoading.SetActive(true);
        canvasGroup.alpha = 0;
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.3f);
        loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadingOperation.isDone)
            yield return null;
        yield return new WaitForSecondsRealtime(0.3f);
        isLoading = false;
        Time.timeScale = 1;
        Pause.canPause = true;
        FadeOut();
    }
    IEnumerator fadeScreen(bool fadeIn)
    {
        if (fadeIn)
        {
            panelLoading.SetActive(true);
            canvasGroup.alpha = 0;
            while(canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            canvasGroup.alpha = 1;
            while(canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime;
                yield return null;
            }
            panelLoading.SetActive(false);
        }
    }
    public void FadeIn()
    {
        if(fadeScreenCoroutine != null)
            StopCoroutine(fadeScreenCoroutine);
        fadeScreenCoroutine = StartCoroutine(fadeScreen(true));
        RuntimeManager.PlayOneShot(openSoundLoading);
    }

    public void FadeOut()
    {
        if(fadeScreenCoroutine != null)
            StopCoroutine(fadeScreenCoroutine);
        fadeScreenCoroutine = StartCoroutine(fadeScreen(false));
        RuntimeManager.PlayOneShot(closeSoundLoading);
    }
}
