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
    float timerReaction;
    float timerToLeavePlayer;
    bool IsSeePlayer;
    bool readyToAtack;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        gun = GetComponentInChildren<Gun>();
        player = enemyMain.target;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyMain == null)
        {
            Destroy(this);
            Destroy(gun);
            return;
        }
        IsSeePlayer = isSeePlayer();
        enemyMain.Move(IsSeePlayer ? EnemyMain.TypeMovement.MoveDodge : EnemyMain.TypeMovement.Move);
        if(CanAtackPlayer())
            gun.Shoot();
    }
    public bool isSeePlayer()
    {
        if(Physics.Raycast(transform.position,(player.position - transform.position).normalized, Mathf.Infinity, gun.surfaceToLook))
        {
            if(IsSeePlayer && timerToLeavePlayer < 1f)
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
