using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    EnemyMain enemyMain;
    Transform player;
    Gun gun;
    [Header("Values:")]
    [SerializeField] float reactionTime = 0.1f;
    [SerializeField] float distanceToBack = 2;
    [SerializeField] float distanceToStay = 6;
    bool backMove;
    float timerToLeavePlayer;
    float timerReaction;
    bool IsSeePlayer;
    bool readyToAtack;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        gun = GetComponentInChildren<Gun>();
        player = enemyMain.target;
        enemyMain.speed = enemyMain.speed + GameManager.instance.stage * 0.15f;
        enemyMain.kdToRandomPath = enemyMain.kdToRandomPath - GameManager.instance.stage * 0.025f;
        gun.kdBeetwenShoots = gun.kdBeetwenShoots - GameManager.instance.stage * 0.015f;
        gun.parametersBullet.force = gun.parametersBullet.force + GameManager.instance.stage * 0.25f;
        reactionTime = reactionTime - GameManager.instance.stage * 0.015f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(enemyMain == null)
        {
            Destroy(this);
            Destroy(gun);
            return;
        }
        IsSeePlayer = isSeePlayer();
        if(backMove == false)
        {
            enemyMain.typeMovement = IsSeePlayer ? EnemyMain.TypeMovement.MoveDodge : EnemyMain.TypeMovement.Move;
            enemyMain.Move();
        }
        else
        {
            enemyMain.typeMovement = EnemyMain.TypeMovement.Move;
            enemyMain.Move(transform.position + (transform.position - player.position).normalized);
        }
        if(Vector3.Distance(transform.position,player.position) <= distanceToBack && backMove == false)
            backMove = true;
        if(Vector3.Distance(transform.position,player.position) >= distanceToStay && backMove)
            backMove = false;
        if(CanAtackPlayer())
            gun.Shoot();
    }
    public bool isSeePlayer()
    {
        if(Physics.Raycast(transform.position,(player.position - transform.position).normalized,out RaycastHit hit, Mathf.Infinity, gun.surfaceToLook) && hit.transform.tag != "Player")
        {
            if(IsSeePlayer && timerToLeavePlayer < 0.5f)
            {
                timerToLeavePlayer += Time.deltaTime;
                return true;
            }
            else
            {
                timerToLeavePlayer = 0;
                return false;
            }
        }
        else
        {
            timerToLeavePlayer = 0;
            return true;
        }
    }
    public bool CanAtackPlayer()
    {
        if (IsSeePlayer)
        {
            if(readyToAtack == false)
            {
                timerReaction = reactionTime;
                readyToAtack = true;
                return false;
            }
        }
        else
        {
            timerReaction = 0;
            readyToAtack = false;
            return false;
        }
        if(timerReaction > 0)
        {
            timerReaction -= Time.deltaTime;
            return false;
        }
        return true;
    }
}
