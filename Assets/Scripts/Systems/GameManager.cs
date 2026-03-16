using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class RoomSettings
    {
        public Room room;
        public int stageOn = 0;
    }
    [Header("Game:")]
    [SerializeField] Player player;
    [SerializeField] RoomSettings[] rooms;
    [SerializeField] RoomSettings[] bossRooms;
    [SerializeField] RoomSettings debugRoom;
    public UnityEvent onStartLevel;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI textCountdown;
    [SerializeField] TextMeshProUGUI textTimer;
    [SerializeField] TextMeshProUGUI textStages;
    [SerializeField] GameObject resultsPanel;
    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject shop;
    [SerializeField] GameObject buttonNext;
    public int stage;
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
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        TicTacToeManager.instance.onChooseCell.AddListener(StartLevel);
        UpdateTextStage();
        ComboManager.instance.SetupCombo();
        SetupRoomsPool();
    }
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
        TicTacToeManager.instance.SetTicTacToe(true);
        TicTacToeManager.instance.SetupTicTacToe();
    }
    void UpdateTextStage()=>textStages.text = $"STAGE:{stage}";
    public void StartLevel(int idCell)
    {
        StartCoroutine(startGameCountdown());
        currentChoosenCell = idCell;
        gameIsStarted = true;
        isBossFight = TicTacToeManager.instance.TryMove(idCell) == TicTacToeManager.Winner.Player;
        gameUI.SetActive(true);
        TicTacToeManager.instance.SetTicTacToe(false);
        DestroyAllRooms();
        SetupRoom();
        onStartLevel.Invoke();
    }
    IEnumerator startGameCountdown()
    {
        Time.timeScale = 0;
        textCountdown.gameObject.SetActive(true);
        textCountdown.text = "<color=green>3</color>";
        yield return new WaitForSecondsRealtime(0.5f - stage * 0.05f);
        textCountdown.text = "<color=yellow>2</color>";
        yield return new WaitForSecondsRealtime(0.5f - stage * 0.05f);
        textCountdown.text = "<color=red>1</color>";
        yield return new WaitForSecondsRealtime(0.5f - stage * 0.05f);
        textCountdown.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    void Update()
    {
        if(gameIsStarted == false || isBossFight)
            return;
        if(timer > 0)
            timer-= Time.deltaTime - BuffManager.instance.passiveBuff.lessTimeFade;
        else
            EndLevel(TicTacToeManager.Winner.Enemy);
        textTimer.text = timer.ToString("F2");
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
    public void ClearBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for(int i = 0; i < bullets.Length;i++)
            bullets[i].SetActive(false);
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
        if(gameIsStarted == false)
            return;
        gameUI.SetActive(false);
        gameIsStarted = false;
        for(int i = 0;i < healthBars.Count; i++)
            UIManagerGame.instance.RemoveHealthBar(healthBars[i]);
        healthBars.Clear();
        for(int i = 0;i < enemies.Count;i++)
            Destroy(enemies[i].gameObject);
        enemies.Clear();
        ClearBullets();
        Destroy(currentPlayer);
        TicTacToeManager.instance.SetTicTacToe(true);
        switch (winner)
        {
            case TicTacToeManager.Winner.Player:
                TicTacToeManager.instance.whatsTurn = TicTacToeManager.Winner.Player;
                TicTacToeManager.instance.PlayCell(currentChoosenCell);
            break;
            case TicTacToeManager.Winner.Enemy:
                TicTacToeManager.instance.whatsTurn = TicTacToeManager.Winner.Enemy;
                TicTacToeManager.instance.MoveEnemy();
            break;
        }
        if(TicTacToeManager.instance.currentWinner != TicTacToeManager.Winner.None)
        {
            switch (TicTacToeManager.instance.currentWinner)
            {
                case TicTacToeManager.Winner.Enemy:
                    textStages.text = "YOU LOSE";
                break;
                case TicTacToeManager.Winner.Tie:
                    textStages.text = "THIS TIE BUT YOU STILL LOSE";
                break;
            }
            buttonNext.SetActive(true);
        }
        DestroyAllRooms();
    }
    public void Next()
    {
        switch (TicTacToeManager.instance.currentWinner)
        {
            case TicTacToeManager.Winner.Player:
                shop.SetActive(true);
            break;
            default:
                resultsPanel.SetActive(true);
                ScoreManager.instance.setInfo();
                BuffManager.instance.cheakUIEndContent();
            break;
        }
        TicTacToeManager.instance.SetTicTacToe(false);
    }
}
