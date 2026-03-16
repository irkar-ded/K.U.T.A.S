using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    public enum FirstBossStates
    {
        Idle,
        BulletHell,
        EnemyAtack
    }
    [Header("Values:")]
    [SerializeField] FirstBossStates bossState;
    FirstBossStates currentBossState;
    [Header("Shoot State:")]
    [SerializeField] GameObject bullet;
    [SerializeField] Bullet.ParametersBullet parametersBullet;
    [SerializeField] Transform spawnPointToBulletParent;
    [SerializeField] Transform[] spawnPointsToBullet;
    [Header("Spawn Enemey State:")]
    [SerializeField] EnemySpawner[] enemySpawners;
    EnemyMain enemyMain;
    Transform player;
    int aliveEnemies;
    Coroutine currentWorkState;
    Room room;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        room = FindObjectOfType<Room>();
        enemyMain = GetComponent<EnemyMain>();
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + GameManager.instance.stage * 5;
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
        enemyMain.healtSystem.onDie.AddListener(() => ComboManager.instance.addCombo(1));
        SetStateBoss(FirstBossStates.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBossState != bossState)
            SetStateBoss(bossState);
        Quaternion rotToPlayer = Quaternion.LookRotation((player.position - transform.position).normalized);
        rotToPlayer.z = 0;
        rotToPlayer.x = 0;
        transform.rotation = rotToPlayer;
    }
    public void SetStateBoss(FirstBossStates state)
    {
        currentBossState = state;
        if(currentWorkState != null)
            StopCoroutine(currentWorkState);
        switch (state)
        {
            case FirstBossStates.Idle:
                currentWorkState = StartCoroutine(idleStateBoss());
            break;
            case FirstBossStates.BulletHell:
                currentWorkState = StartCoroutine(bulletHellStateBoss());
            break;
            case FirstBossStates.EnemyAtack:
                currentWorkState = StartCoroutine(enemyAtackStateBoss());
            break;
        }
    }
    IEnumerator idleStateBoss()
    {
        yield return new WaitForSeconds(1-GameManager.instance.stage * 0.015f);
        bossState = FirstBossStates.BulletHell;
    }
    IEnumerator bulletHellStateBoss()
    {
        yield return new WaitForSeconds(0.5f-GameManager.instance.stage * 0.015f);
        float timer = 0;
        float timerToSpawnBullet = 0;
        float currentRot = 0;
        while(timer <= 10)
        {
            timer+=Time.deltaTime;
            timerToSpawnBullet += Time.deltaTime;
            currentRot += Time.deltaTime * 30;
            if(timerToSpawnBullet >= 0.2f-GameManager.instance.stage * 0.01f)
            {
                spawnPointToBulletParent.rotation = Quaternion.Euler(0,currentRot,0);
                for(int i = 0;i < spawnPointsToBullet.Length; i++)
                    EZ_PoolManager.Spawn(bullet.transform, spawnPointsToBullet[i].position, spawnPointsToBullet[i].rotation).GetComponent<Bullet>().SetBullet(parametersBullet);
                currentRot++;
                timerToSpawnBullet = 0;
            }
            yield return null;
        }
        bossState = FirstBossStates.EnemyAtack;
    }
    IEnumerator enemyAtackStateBoss()
    {
        yield return new WaitForSeconds(0.5f-GameManager.instance.stage * 0.015f);
        for(int i = 0; i < enemySpawners.Length; i++)
        {
            EnemyMain enemy = enemySpawners[i].SpawnEnemy();
            print(enemy.gameObject.name);
            enemy.GetComponent<HealtSystem>().onDie.AddListener(OnDieEnemy);
            room.AddEnemy(enemy,false);
            aliveEnemies++;
            print(aliveEnemies);
        }
        float timer = 0;
        while(timer <= 10 && aliveEnemies > 0)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        bossState = FirstBossStates.BulletHell;
    }
    public void OnDieEnemy()=>aliveEnemies--;
}
