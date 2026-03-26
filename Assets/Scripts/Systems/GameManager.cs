using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class RoomSettings
    {
        public Room room;
        public int stageOn = 0;
    }
    [Header("Game:")]
    public Transform playerPositionCamera;
    public Transform mousePositionCamera;
    [SerializeField] Player player;
    [SerializeField] RoomSettings[] rooms;
    [SerializeField] RoomSettings[] bossRooms;
    [SerializeField] RoomSettings debugRoom;
    public UnityEvent onStartLevel;
    [Header("Beetwen Game Content")]
    [SerializeField] GameObject beetwenGameBackground;
    [SerializeField] GameObject beetwenGameCamera;
    [Header("UI")]
    [SerializeField] MovePanelAnimation movePanelAnimation;
    [SerializeField] TextMeshProUGUI textCountdown;
    [SerializeField] TextMeshProUGUI textTimer;
    [SerializeField] TextMeshProUGUI textStages;
    [SerializeField] Animator blackLines;
    [SerializeField] GameObject bossOrbitalCamera;
    [SerializeField] TextMeshProUGUI bossText;
    [SerializeField] CanvasGroup resultsPanel;
    [SerializeField] GameObject otherUI;
    [SerializeField] CanvasGroup ticTacToeUI;
    [SerializeField] CanvasGroup gameUI;
    [SerializeField] CanvasGroup shop;
    [SerializeField] GameObject buttonNext;
    [Header("Colors")]
    [SerializeField] Color colorNumberOne;
    [SerializeField] Color colorNumberTwo;
    [SerializeField] Color colorNumberThree;
    [Header("Sound")]
    [SerializeField] EventReference soundTick;
    [SerializeField] EventReference soundTyping;
    [SerializeField] EventReference soundBackspace;
    [SerializeField] EventReference soundClose;
    [SerializeField] EventReference soundOpen;
    [SerializeField] EventReference soundVHSDeath;
    bool isBossCutscene;
    public int stage;
    [HideInInspector]public float difficulty;
    public static GameManager instance;
    List<GameObject> enemies = new List<GameObject>();
    List<RoomSettings> currentRoomPool = new List<RoomSettings>();
    List<RoomSettings> currentBossRoomPool = new List<RoomSettings>();
    List<HealthBar> healthBars = new List<HealthBar>();
    RoomSettings lastBossRoom;
    RoomSettings lastRoom;
    public bool gameIsStarted;
    bool isBossFight;
    int currentChoosenCell;
    float timer;
    Room currentRoom;
    GameObject currentPlayer;
    Controls gameInputs;
    InputAction pauseKey;
    [HideInInspector]public bool endGameState;
    // Start is called before the first frame update
    void Awake()=>instance = this;
    void Start()
    {
        TicTacToeManager.instance.onChooseCell.AddListener(StartLevel);
        UpdateTextStage();
        ComboManager.instance.SetupCombo();
        SetupRoomsPool();
        if (SettingsManager.instance != null)
            gameInputs = SettingsManager.gameInputs;
        else
            gameInputs = new Controls();
        pauseKey = gameInputs.Player.Pause;
        pauseKey.Enable();
    }
    void OnDisable()=>pauseKey.Disable();
    void SetupRoomsPool()
    {
        SetupBossRoomPool();
        SetupRoomPool();
    }
    void SetupBossRoomPool() => currentBossRoomPool = new List<RoomSettings>(bossRooms);
    void SetupRoomPool() => currentRoomPool = new List<RoomSettings>(rooms);
    public void NextStage()
    {
        stage++;
        UpdateTextStage();
        ComboManager.instance.SetupCombo();
        buttonNext.SetActive(false);
        gameIsStarted = false;
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(ticTacToeUI,shop));
        TicTacToeManager.instance.SetupTicTacToe();
    }
    void UpdateTextStage()=>textStages.text = $"STAGE:{stage}";
    public void StartLevel(int idCell)
    {
        difficulty = stage * 0.5f;
        RuntimeManager.PlayOneShot(soundClose);
        ClearMap();
        StartCoroutine(beetwenGameOutCutscene());
        Pause.canPause = false;
        currentChoosenCell = idCell;
        isBossFight = TicTacToeManager.instance.TryMove(idCell) == TicTacToeManager.Winner.Player;
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(gameUI,ticTacToeUI));
        DestroyAllRooms();
        SetupRoom();
        if(isBossFight == false)
            StartCoroutine(startGameCountdown());
        else
            StartCoroutine(startBossFightCutscene());
        onStartLevel.Invoke();
    }
    IEnumerator beetwenGameOutCutscene()
    {
        beetwenGameCamera.SetActive(false);
        yield return new WaitForSecondsRealtime(1);
        beetwenGameBackground.SetActive(false);
    }
    IEnumerator startBossFightCutscene()
    {
        StartCoroutine(textBossNameCutscene());
        isBossCutscene = true;
        bossOrbitalCamera.SetActive(true);
        blackLines.SetInteger("FadeState",1);
        yield return new WaitForSeconds(4f);
        bossOrbitalCamera.SetActive(false);
        blackLines.SetInteger("FadeState",-1);
        isBossCutscene = false;
        StartCoroutine(startGameCountdown());
    }
    IEnumerator textBossNameCutscene()
    {
            bossText.gameObject.SetActive(true);
            string final = currentRoom.getNameEnemy(0);
            bossText.text = "";
            string currentText = "";
            for (int i = 0; i < final.Length; i++)
            {
                currentText += final[i];
                if (final[i] == ' ')
                    continue;
                bossText.text = currentText + "_";
                RuntimeManager.PlayOneShot(soundTyping);
                yield return new WaitForSeconds(0.075f);
            }
            bossText.text = currentText;
            yield return new WaitForSeconds(2f);
            currentText = final;
            while (currentText.Length > 0)
            {
                currentText = currentText.Remove(currentText.Length - 1,1);
                if(currentText.Length <= 0)
                    break;
                if (currentText[currentText.Length - 1] == ' ')
                {
                    bossText.text = currentText;
                    continue;
                }
                bossText.text = currentText + "_";
                RuntimeManager.PlayOneShot(soundBackspace);
                yield return new WaitForSeconds(0.075f);
            }
            bossText.text = currentText;
            bossText.gameObject.SetActive(false);
    }
    IEnumerator startGameCountdown()
    {
        yield return new WaitForSeconds(0.25f);
        textCountdown.gameObject.SetActive(true);
        textCountdown.color = colorNumberThree;
        textCountdown.text = "3";
        RuntimeManager.PlayOneShot(soundTick,Vector3.zero);
        yield return new WaitForSeconds(0.5f);
        textCountdown.color = colorNumberTwo;
        textCountdown.text = "2";
        RuntimeManager.PlayOneShot(soundTick,Vector3.zero);
        yield return new WaitForSeconds(0.5f);
        textCountdown.color = colorNumberOne;
        textCountdown.text = "1";
        RuntimeManager.PlayOneShot(soundTick,Vector3.zero);
        yield return new WaitForSeconds(0.5f);
        Pause.canPause = true;
        gameIsStarted = true;
        textCountdown.gameObject.SetActive(false);
    }
    void Update()
    {
        if(pauseKey.WasPerformedThisFrame() && isBossCutscene)
        {
            StopAllCoroutines();
            bossText.gameObject.SetActive(false);
            bossOrbitalCamera.SetActive(false);
            blackLines.SetInteger("FadeState",-1);
            isBossCutscene = false;
            StartCoroutine(startGameCountdown());
        }
        if(gameIsStarted == false || isBossFight || debugRoom != null && debugRoom.room != null)
            return;
        if(timer > 0)
            timer-= Time.deltaTime / (1 + BuffManager.instance.passiveBuff.lessTimeFade);
        else
            EndLevel(TicTacToeManager.Winner.Enemy);
        textTimer.text = $"<color={(timer > 3 ? "white" : "#ff5470")}>{timer.ToString("F2")}</color>";
    }
    public void DestroyAllRooms()
    {
        if(debugRoom != null && debugRoom.room != null)
        {
            debugRoom.room.gameObject.SetActive(false);
            return;
        }
        for(int i = 0;i < rooms.Length;i++)
            rooms[i].room.gameObject.SetActive(false);
        for(int i = 0;i < bossRooms.Length;i++)
            bossRooms[i].room.gameObject.SetActive(false);
    }
    public void SetupRoom()
    {
        if(debugRoom != null && debugRoom.room != null)
        {
            currentRoom = debugRoom.room;
            textTimer.gameObject.SetActive(false);
        }
        else if (isBossFight)
        {
            if(currentBossRoomPool.Count <= 0)
            {
                SetupBossRoomPool();
                currentBossRoomPool.Remove(lastBossRoom);
            }
            RoomSettings roomSettings = currentBossRoomPool[Random.Range(0,currentBossRoomPool.Count)];
            lastBossRoom = roomSettings;
            currentRoom = roomSettings.room;
            currentBossRoomPool.Remove(roomSettings);
            textTimer.gameObject.SetActive(false);
        }
        else
        {
            timer = 10;
            textTimer.text = timer.ToString("F2");
            textTimer.gameObject.SetActive(true);
            if(currentRoomPool.Count <= 0)
            {
                SetupRoomPool();
                currentRoomPool.Remove(lastRoom);
            }
            List<RoomSettings> roomsInPool = new List<RoomSettings>(currentRoomPool);
            roomsInPool.RemoveAll(x => x.stageOn > stage);
            if(roomsInPool.Count <= 0)
            {
                SetupRoomPool();
                currentRoomPool.Remove(lastRoom);
                roomsInPool = new List<RoomSettings>(currentRoomPool);
                roomsInPool.RemoveAll(x => x.stageOn > stage);
            }
            RoomSettings roomSettings = roomsInPool[Random.Range(0,roomsInPool.Count)];
            lastRoom = roomSettings;
            currentRoomPool.Remove(roomSettings);
            currentRoom = roomSettings.room;
        }
        currentRoom.gameObject.SetActive(true);
        currentRoom.PrepareRoom();
    }
    public void ClearMap()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for(int i = 0; i < bullets.Length;i++)
            bullets[i].SetActive(false);
        GameObject[] particles = GameObject.FindGameObjectsWithTag("Particle");
        for(int i = 0; i < particles.Length;i++)
            particles[i].SetActive(false);
    }
    public GameObject SpawnPlayer(Vector3 pos)
    {
        currentPlayer = Instantiate(player,pos,Quaternion.identity).gameObject;
        SetupPlayer();
        return currentPlayer;
    }
    public void SetupPlayer()
    {
        Player tmpPlayer = currentPlayer.GetComponent<Player>();
        Gun gunPlayerStart = tmpPlayer.GetComponentInChildren<Gun>();
        Gun gunPlayer = tmpPlayer.GetComponentInChildren<Gun>();
        HealtSystem playerHealth = tmpPlayer.GetComponent<HealtSystem>();
        gunPlayer.kdBeetwenShoots = gunPlayerStart.kdBeetwenShoots - BuffManager.instance.passiveBuff.fireRate;
        gunPlayer.parametersBullet.force = gunPlayerStart.parametersBullet.force + BuffManager.instance.passiveBuff.bonusForceBullet;
        gunPlayer.parametersBullet.damage = gunPlayerStart.parametersBullet.damage + BuffManager.instance.passiveBuff.damage;
        gunPlayer.parametersBullet.bounceBullet = BuffManager.instance.passiveBuff.bounceBullet;
        gunPlayer.parametersBullet.xRayBullet = BuffManager.instance.passiveBuff.xRayBullet;
        gunPlayer.parametersBullet.toxicBullet = BuffManager.instance.passiveBuff.toxicBullet;
        playerHealth.maxHealt = 1 + BuffManager.instance.passiveBuff.maxHealth;
        playerHealth.healt = playerHealth.maxHealt;
        tmpPlayer.speed = player.speed + BuffManager.instance.passiveBuff.bonusSpeed;
        HealthCells.instance.SetupHealthCells(playerHealth);
    }
    public void AddEnemy(GameObject enemy,HealthBar healthBar)
    {
        if(healthBar != null)
            healthBars.Add(healthBar);
        enemies.Add(enemy);
    }
    public void EndLevel(TicTacToeManager.Winner winner)
    {
        if(gameIsStarted == false || endGameState)
            return;
        endGameState = true;
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(null,gameUI));
        Pause.canPause = false;
        Time.timeScale = 0.25f;
        for(int i = 0;i < healthBars.Count; i++)
            UIManagerGame.instance.RemoveHealthBar(healthBars[i]);
        healthBars.Clear();
        if(winner == TicTacToeManager.Winner.Enemy)
        {
            if(PostEffectsManager.instance != null)
                PostEffectsManager.instance.SetBackAndWhite(true);
            RuntimeManager.PlayOneShot(soundVHSDeath);
        }
        currentPlayer.GetComponent<DeadPlayer>().MakeAlwaysInvincible();
        StartCoroutine(waitToEndLevel(winner));
    }
    IEnumerator waitToEndLevel(TicTacToeManager.Winner winner)
    {
        yield return new WaitForSecondsRealtime(isBossFight || winner == TicTacToeManager.Winner.Enemy ? 3f : 2f);
        timer = 10;
        beetwenGameCamera.SetActive(true);
        beetwenGameBackground.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        switch (winner)
        {
            case TicTacToeManager.Winner.Player:
                TicTacToeManager.instance.whatsTurn = TicTacToeManager.Winner.Player;
                TicTacToeManager.instance.PlayCell(currentChoosenCell);
            break;
            case TicTacToeManager.Winner.Enemy:
                if(PostEffectsManager.instance != null)
                    PostEffectsManager.instance.SetBackAndWhite(true);
                TicTacToeManager.instance.whatsTurn = TicTacToeManager.Winner.Enemy;
                TicTacToeManager.instance.MoveEnemy();
            break;
        }
        if(PostEffectsManager.instance != null && winner == TicTacToeManager.Winner.Enemy)
            PostEffectsManager.instance.SetBackAndWhite(false);
        endGameState = false;
        gameIsStarted = false;
        ClearMap();
        for(int i = 0;i < enemies.Count;i++)
            Destroy(enemies[i].gameObject);
        enemies.Clear();
        Destroy(currentPlayer);
        movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(ticTacToeUI,null));
        if(TicTacToeManager.instance.currentWinner != TicTacToeManager.Winner.None)
        {
            switch (TicTacToeManager.instance.currentWinner)
            {
                case TicTacToeManager.Winner.Player:
                    textStages.text = "YOU WIN";
                break;
                case TicTacToeManager.Winner.Enemy:
                    textStages.text = "YOU LOSE";
                break;
                case TicTacToeManager.Winner.Tie:
                    textStages.text = "THIS TIE BUT YOU STILL LOSE :)";
                break;
            }
            buttonNext.SetActive(true);
        }
        DestroyAllRooms();
        Time.timeScale = 1f;
        Pause.canPause = true;
        RuntimeManager.PlayOneShot(soundOpen);
    }
    public void Next()
    {
        switch (TicTacToeManager.instance.currentWinner)
        {
            case TicTacToeManager.Winner.Player:
                movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(shop,ticTacToeUI));
            break;
            default:
                movePanelAnimation.MovePanel(new MovePanelAnimation.Transition(resultsPanel,ticTacToeUI));
                otherUI.SetActive(false);
                ScoreManager.instance.setInfo();
                BuffManager.instance.cheakUIEndContent();
            break;
        }
    }
}
