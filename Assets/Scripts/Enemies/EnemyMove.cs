using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    EnemyMain enemyMain;
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        enemyMain.speed = enemyMain.speed + GameManager.instance.stage * 0.15f;
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + GameManager.instance.stage * 0.25f;
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
    }
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(enemyMain == null)
        {
            Destroy(this);
            return;
        }
        enemyMain.Move();
    }
}
