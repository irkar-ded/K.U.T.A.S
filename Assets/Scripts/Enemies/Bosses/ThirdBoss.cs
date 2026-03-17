using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBoss : MonoBehaviour
{
    public enum ThirdBossStates
    {
        Idle,
        Shoot,
        Dash
    }
    [Header("Values:")]
    [SerializeField] ThirdBossStates bossState;
    [SerializeField] float distanceToBack = 2;
    [SerializeField] float distanceToStay = 6;
    bool backMove;
    ThirdBossStates currentBossState;
    float startSpeed;
    EnemyMain enemyMain;
    Transform player;
    Rigidbody rbPlayer;
    Coroutine currentWorkState;
    Gun gun;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        gun = GetComponentInChildren<Gun>();
        enemyMain.healtSystem.onDie.AddListener(() => ComboManager.instance.addCombo(1));
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + GameManager.instance.stage * 5;
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
        enemyMain.speed = enemyMain.speed + GameManager.instance.stage * 0.1f;
        enemyMain.kdToRandomPath = enemyMain.kdToRandomPath - GameManager.instance.stage * 0.015f;
        gun.kdBeetwenShoots = gun.kdBeetwenShoots - GameManager.instance.stage * 0.01f;
        gun.parametersBullet.force = gun.parametersBullet.force + GameManager.instance.stage * 0.1f;
        startSpeed = enemyMain.speed;
        player = enemyMain.target;
        rbPlayer = player.GetComponent<Rigidbody>();
        SetStateBoss(ThirdBossStates.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(currentBossState != bossState)
            SetStateBoss(bossState);
        /*Quaternion rotToPlayer = Quaternion.LookRotation((player.position - transform.position).normalized);
        rotToPlayer.z = 0;
        rotToPlayer.x = 0;
        transform.rotation = rotToPlayer;*/
    }
    public void SetStateBoss(ThirdBossStates state)
    {
        enemyMain.agent.enabled = true;
        enemyMain.agent.isStopped = false;
        enemyMain.speed = startSpeed;
        currentBossState = state;
        if(currentWorkState != null)
            StopCoroutine(currentWorkState);
        switch (state)
        {
            case ThirdBossStates.Idle:
                currentWorkState = StartCoroutine(idleStateBoss());
            break;
            case ThirdBossStates.Shoot:
                currentWorkState = StartCoroutine(shootStateBoss());
            break;
            case ThirdBossStates.Dash:
                currentWorkState = StartCoroutine(dashStateBoss());
            break;
        }
    }
    IEnumerator idleStateBoss()
    {
        yield return new WaitForSeconds(1-GameManager.instance.stage * 0.015f);
        bossState = ThirdBossStates.Shoot;
    }
    IEnumerator shootStateBoss()
    {
        float timer = 0;
        while(timer <= 2.5f)
        {
            if(backMove == false)
                enemyMain.Move();
            else
                enemyMain.Move(transform.position + (transform.position - player.position).normalized);
            if(Vector3.Distance(transform.position,player.position) <= distanceToBack && backMove == false)
                backMove = true;
            if(Vector3.Distance(transform.position,player.position) >= distanceToStay && backMove)
                backMove = false;
            gun.Shoot();
            timer += Time.deltaTime;
            yield return null;
        }
        bossState = ThirdBossStates.Dash;
    }
    IEnumerator dashStateBoss()
    {
        enemyMain.agent.isStopped = true;
        enemyMain.agent.enabled = false;
        Quaternion rotToPlayer = Quaternion.LookRotation((player.position - transform.position).normalized + rbPlayer.velocity.normalized);
        rotToPlayer.z = 0;
        rotToPlayer.x = 0;
        transform.rotation = rotToPlayer;
        Vector3 target = player.position + rbPlayer.velocity.normalized;
        yield return new WaitForSeconds(0.15f - GameManager.instance.stage * 0.015f);
        enemyMain.agent.enabled = true;
        enemyMain.agent.isStopped = false;
        enemyMain.speed = 50;
        float timer = 0;
        while(timer <= 1 && Vector3.Distance(transform.position,target) > 1)
        {
            enemyMain.Move(target);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.1f - GameManager.instance.stage * 0.015f);
        bossState = ThirdBossStates.Shoot;
    }
}
