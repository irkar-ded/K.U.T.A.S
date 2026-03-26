using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Values:")]
    [SerializeField] Transform[] spawnPointsPlayer;
    [SerializeField] EnemySpawner[] enemySpawners;
    List<EnemyMain> enemies = new List<EnemyMain>();
    int coutAliveEnemies = 0;
    public void PrepareRoom()
    {
        enemies.Clear();
        coutAliveEnemies = 0;
        HealtSystem healtPlayer = GameManager.instance.SpawnPlayer(spawnPointsPlayer[Random.Range(0,spawnPointsPlayer.Length)].position).GetComponent<HealtSystem>();
        healtPlayer.onDie.AddListener(() => GameManager.instance.EndLevel(TicTacToeManager.Winner.Enemy));
        for(int i = 0;i < enemySpawners.Length; i++)
        {
            EnemyMain enemy = enemySpawners[i].SpawnEnemy();
            if(enemy != null)
                AddEnemy(enemy,enemySpawners[i].useHealthbar);
        }
    }
    public void AddEnemy(EnemyMain enemy,bool useHealthbar)
    {
        GameManager.instance.AddEnemy(enemy.gameObject,useHealthbar ? UIManagerGame.instance.CreateHealthBar(enemy.healtSystem,enemy.nameEnemy) : null);
        enemy.GetComponent<HealtSystem>().onDie.AddListener(() => OnDieEnemy(enemy));
        TargetHolder.instance.CreatePoint(enemy.transform);
        enemies.Add(enemy);
        coutAliveEnemies++;
    }
    public void OnDieEnemy(EnemyMain enemy)
    {
        coutAliveEnemies--;
        if(coutAliveEnemies <= 0)
            GameManager.instance.EndLevel(TicTacToeManager.Winner.Player);
        TargetHolder.instance.DestroyPoint(enemy.transform);
    }
    public string getNameEnemy(int id) => enemies[id].nameEnemy;
}
