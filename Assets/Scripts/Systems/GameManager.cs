using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class RoomSettings
    {
        public Room room;
        public int stageOn = 0;
    }
    [Header("Game:")]
    [SerializeField] GameObject player;
    [SerializeField] RoomSettings[] rooms;
    [SerializeField] RoomSettings[] bossRooms;
    [SerializeField] RoomSettings debugRoom;
    public int stage;
    public static GameManager instance;
    List<GameObject> enemies = new List<GameObject>();
    public bool gameIsStarted;
    bool isBossFight;
    int currentChoosenCell;
    Room currentRoom;
    GameObject currentPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }
    void Start()=>TicTacToeManager.instance.onChooseCell.AddListener(StartLevel);
    public void StartLevel(int idCell)
    {
        currentChoosenCell = idCell;
        gameIsStarted = true;
        isBossFight = TicTacToeManager.instance.TryMove(idCell) == TicTacToeManager.Winner.Player;
        TicTacToeManager.instance.SetTicTacToe(false);
        DestroyAllRooms();
        SetupRoom();
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
            currentRoom = debugRoom.room;
        else if(isBossFight)
            currentRoom = bossRooms[Random.Range(0,bossRooms.Length)].room;
        else
        {
            List<RoomSettings> roomsInPool = rooms.ToList();
            roomsInPool.RemoveAll(x => x.stageOn > stage);
            currentRoom = roomsInPool[Random.Range(0,roomsInPool.Count)].room;
        }
        currentRoom.gameObject.SetActive(true);
        currentRoom.PrepareRoom();
    }
    public GameObject SpawnPlayer(Vector3 pos)=>currentPlayer = Instantiate(player,pos,Quaternion.identity);
    public void AddEnemy(GameObject enemy) => enemies.Add(enemy);
    public void EndLevel(TicTacToeManager.Winner winner)
    {
        if(gameIsStarted == false)
            return;
        gameIsStarted = false;
        for(int i = 0;i < enemies.Count;i++)
            Destroy(enemies[i].gameObject);
        enemies.Clear();
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
        DestroyAllRooms();
    }
}
