using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    TextMeshProUGUI scoreText;
    TextMeshProUGUI timerText;
    TextMeshProUGUI killText;
    [SerializeField] int score;
    [SerializeField] float timer;
    [SerializeField] int kill;
    [HideInInspector] public UnityEvent onKill;
    public static ScoreManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void addScore(int add)=>score += add;
    public void addKill()
    {
        kill++;
        onKill.Invoke();
    }
    private void Update()
    {
        /*if (GameManager.gameAllReady == false)
            return;*/
        timer += Time.deltaTime;
    }
    public void setInfo()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int milliseconds = Mathf.FloorToInt((timer * 100f) % 100);
        string formattedTime = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
        scoreText.text = $"SCORE:{score.ToString("D4")}";
        killText.text = $"KILL:{kill.ToString("D3")}";
        timerText.text = $"TIME:{formattedTime}";
    }

}
