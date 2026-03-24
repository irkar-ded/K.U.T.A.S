using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using FMODUnity;
using UnityEngine;

public class SecondBoss : MonoBehaviour
{
    public enum SecondBossStates
    {
        Idle,
        BulletHell,
        Explosion,
        Teleport
    }
    [Header("Values:")]
    [SerializeField] SecondBossStates bossState;
    SecondBossStates currentBossState;
    [Header("Explosion State:")]
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject markToExplosion;
    [Header("Bullet Hell:")]
    [SerializeField] GameObject bullet;
    [SerializeField] Bullet.ParametersBullet parametersBullet;
    [Header("Sound")]
    [SerializeField] EventReference soundAtackExplosion;
    bool isLastExplosion;
    float currentHealthToTeleport;
    HealtSystem healtSystem;
    Coroutine currentWorkState;
    List<Transform> teleportPoints = new List<Transform>();
    EnemyMain enemyMain;
    Animator anim;
    Transform player;
    Rigidbody rbPlayer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        rbPlayer = player.GetComponent<Rigidbody>();
        enemyMain = GetComponent<EnemyMain>();
        healtSystem = enemyMain.healtSystem;
        healtSystem.maxHealt = healtSystem.maxHealt + GameManager.instance.stage * 5;
        anim.speed *= 1 + GameManager.instance.stage * 0.25f;
        healtSystem.healt = healtSystem.maxHealt;
        healtSystem.onDie.AddListener(() => ComboManager.instance.addCombo(1));
        currentHealthToTeleport = healtSystem.healt;
        healtSystem.onTakeDamage.AddListener((Vector3) => CheckToTeleport());
        for(int i = 1;i < 5; i++)
            teleportPoints.Add(GameObject.Find($"TeleportSecondBossPoint{i}").transform);
        SetStateBoss(SecondBossStates.Idle);
    }
    public void CheckToTeleport()
    {
        if(healtSystem.healt + 20 + GameManager.instance.stage * 0.015f <= currentHealthToTeleport)
        {
            currentHealthToTeleport = healtSystem.healt;
            bossState = SecondBossStates.Teleport;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(enemyMain == null)
        {
            markToExplosion.SetActive(false);
            anim.SetTrigger("Death");
            Destroy(this);
            if(currentWorkState != null)
                StopCoroutine(currentWorkState);
            return;
        }
        if(currentBossState != bossState)
            SetStateBoss(bossState);
    }
    public void SetStateBoss(SecondBossStates state)
    {
        markToExplosion.SetActive(false);
        anim.SetBool("Atack",false);
        currentBossState = state;
        if(currentWorkState != null)
            StopCoroutine(currentWorkState);
        switch (state)
        {
            case SecondBossStates.Idle:
                currentWorkState = StartCoroutine(idleStateBoss());
            break;
            case SecondBossStates.BulletHell:
                currentWorkState = StartCoroutine(bulletHellStateBoss());
            break;
            case SecondBossStates.Explosion:
                currentWorkState = StartCoroutine(explosionStateBoss());
            break;
            case SecondBossStates.Teleport:
                anim.SetTrigger("Teleport");
            break;
        }
    }
    IEnumerator idleStateBoss()
    {
        yield return new WaitForSeconds(1-GameManager.instance.stage * 0.01f);
        bossState = SecondBossStates.Teleport;
    }
    public void Teleport()
    {
        List<Transform> tempPointsToTeleport = new List<Transform>(teleportPoints);
        tempPointsToTeleport.RemoveAll(x => Vector3.Distance(x.position,player.position) <= 5f);
        tempPointsToTeleport.RemoveAll(x => Vector3.Distance(x.position,transform.position) <= 1f);
        Transform currentTeleportPoint = tempPointsToTeleport[Random.Range(0,tempPointsToTeleport.Count)];
        transform.position = currentTeleportPoint.position;
        transform.rotation = currentTeleportPoint.rotation;
        bossState = isLastExplosion ? SecondBossStates.BulletHell : SecondBossStates.Explosion;
    }
    IEnumerator bulletHellStateBoss()
    {
        isLastExplosion = false;
        yield return new WaitForSeconds(0.5f-GameManager.instance.stage * 0.01f);
        float timer = 0;
        float timerToSpawnBullet = 0;
        bool isRevers = false;
        float rotationOffset = -5;
        anim.SetBool("Atack",true);
        while(timer <= 10)
        {
            timer+=Time.deltaTime;
            timerToSpawnBullet += Time.deltaTime;
            if(isRevers)
                rotationOffset+=Time.deltaTime * 10;
            else
                rotationOffset-=Time.deltaTime * 10;
            if(rotationOffset >= 20 && isRevers)
                isRevers = false;
            if(rotationOffset <= -20 && isRevers == false)
                isRevers = true;
            if(timerToSpawnBullet >= 0.2f)
            {
                for(int i = -90;i < 180; i++)
                {
                    if(i % 20 == 1)
                        EZ_PoolManager.Spawn(bullet.transform, transform.position, Quaternion.Euler(0,transform.eulerAngles.y + i - 85 + rotationOffset ,0)).GetComponent<Bullet>().SetBullet(parametersBullet);
                }
                timerToSpawnBullet = 0;
            }
            yield return null;
        }
        bossState = SecondBossStates.Explosion;
    }
    IEnumerator explosionStateBoss()
    {
        isLastExplosion = true;
        yield return new WaitForSeconds(1f-GameManager.instance.stage * 0.01f);
        for(int i = 0; i < 10; i++)
        {
            RuntimeManager.PlayOneShot(soundAtackExplosion,transform.position);
            anim.SetBool("Atack",true);
            Vector3 randomPositionExplosion = player.position + rbPlayer.velocity.normalized * 2.5f ;
            randomPositionExplosion.y = -1;
            markToExplosion.transform.position = randomPositionExplosion;
            markToExplosion.SetActive(true);
            yield return new WaitForSeconds(0.65f-GameManager.instance.stage * 0.01f);
            anim.SetBool("Atack",false);
            EZ_PoolManager.Spawn(explosion.transform,randomPositionExplosion + Vector3.up * 0.5f,Quaternion.identity);
            markToExplosion.SetActive(false);
            yield return new WaitForSeconds(0.5f-GameManager.instance.stage * 0.01f);
        }
        bossState = SecondBossStates.BulletHell;
    }
}
