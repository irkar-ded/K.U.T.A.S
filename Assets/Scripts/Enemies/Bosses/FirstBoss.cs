using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using FMODUnity;
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
    [SerializeField] GameObject VFXSpawnEnemy;
    [SerializeField] EnemySpawner[] enemySpawners;
    [Header("Sound")]
    [SerializeField] EventReference soundAtack;
    [SerializeField] EventReference soundSpawnEnemy;
    Animator anim;
    EnemyMain enemyMain;
    Transform player;
    int aliveEnemies;
    Coroutine currentWorkState;
    Room room;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        room = FindObjectOfType<Room>();
        enemyMain = GetComponent<EnemyMain>();
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + GameManager.instance.difficulty * 15;
        anim.speed *= 1 + GameManager.instance.difficulty * 0.15f;
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
        enemyMain.healtSystem.onDie.AddListener(() => ComboManager.instance.addCombo(1));
        SetStateBoss(FirstBossStates.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(enemyMain == null)
        {
            anim.SetTrigger("Death");
            Destroy(this);
            if(currentWorkState != null)
                StopCoroutine(currentWorkState);
            return;
        }
        if(currentBossState != bossState)
            SetStateBoss(bossState);
        Quaternion rotToPlayer = Quaternion.LookRotation((player.position - transform.position).normalized);
        rotToPlayer.z = 0;
        rotToPlayer.x = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation,rotToPlayer,Time.deltaTime * 10);
    }
    public void SetStateBoss(FirstBossStates state)
    {
        anim.SetBool("Shoot",false);
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
        yield return new WaitForSeconds(1-GameManager.instance.difficulty * 0.015f);
        bossState = FirstBossStates.BulletHell;
    }
    IEnumerator bulletHellStateBoss()
    {
        yield return new WaitForSeconds(0.5f-GameManager.instance.difficulty * 0.015f);
        float timer = 0;
        float timerToSpawnBullet = 0;
        float currentRot = 0;
        anim.SetBool("Shoot",true);
        while(timer <= 10)
        {
            timer+=Time.deltaTime;
            timerToSpawnBullet += Time.deltaTime;
            currentRot += Time.deltaTime * (30 * (1 + GameManager.instance.difficulty * 0.1f));
            if(timerToSpawnBullet >= 0.2f-GameManager.instance.difficulty * 0.01f)
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
    public void PlaySoundAtack()=>RuntimeManager.PlayOneShot(soundAtack,transform.position);
    public void EndShootTriggerAnimation() => anim.SetBool("ShootTrigger", false);
    IEnumerator enemyAtackStateBoss()
    {
        yield return new WaitForSeconds(0.5f-GameManager.instance.difficulty * 0.015f);
        anim.SetBool("ShootTrigger",true);
        yield return new WaitForSeconds(0.3f);
        List<Transform> tempsVFXSpawnEnemy = new List<Transform>();
        for(int i = 0; i < enemySpawners.Length; i++)
            tempsVFXSpawnEnemy.Add(EZ_PoolManager.Spawn(VFXSpawnEnemy.transform,enemySpawners[i].transform.position - Vector3.up *1.5f,Quaternion.Euler(-90,0,0)));
        yield return new WaitForSeconds(0.75f);
        RuntimeManager.PlayOneShot(soundSpawnEnemy,transform.position);
        yield return new WaitForSeconds(0.25f);
        for(int i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawners[i].transform.position = tempsVFXSpawnEnemy[i].transform.position + Vector3.up * 1.5f;
            EnemyMain enemy = enemySpawners[i].SpawnEnemy();
            print(enemy.gameObject.name);
            enemy.GetComponent<HealtSystem>().onDie.AddListener(OnDieEnemy);
            room.AddEnemy(enemy,false);
            aliveEnemies++;
            print(aliveEnemies);
        }
        yield return new WaitForSeconds(0.25f);
        float timer = 0;
        while(timer <= 9.75f && aliveEnemies > 0)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        bossState = FirstBossStates.BulletHell;
    }
    public void OnDieEnemy()=>aliveEnemies--;
}
