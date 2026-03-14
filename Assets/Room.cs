using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] Transform[] spawnPointsPlayer;
    [SerializeField] EnemySpawner[] enemySpawners;
    int coutAliveEnemies = 0;
    public void PrepareRoom()
    {
        coutAliveEnemies = 0;
        HealtSystem healtPlayer = GameManager.instance.SpawnPlayer(spawnPointsPlayer[Random.Range(0,spawnPointsPlayer.Length)].position).GetComponent<HealtSystem>();
        healtPlayer.onDie.AddListener(() => GameManager.instance.EndLevel(TicTacToeManager.Winner.Enemy));
        for(int i = 0;i < enemySpawners.Length; i++)
        {
            EnemyMain enemy = enemySpawners[i].SpawnEnemy();
            if(enemy != null)
            {
                GameManager.instance.AddEnemy(enemy.gameObject);
                enemy.GetComponent<HealtSystem>().onDie.AddListener(OnDieEnemy);
                coutAliveEnemies++;
            }
        }
    }
    public void OnDieEnemy()
    {
        coutAliveEnemies--;
        if(coutAliveEnemies <= 0)
            GameManager.instance.EndLevel(TicTacToeManager.Winner.Player);
    }
}
