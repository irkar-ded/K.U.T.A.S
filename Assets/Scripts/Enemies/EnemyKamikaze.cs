using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using UnityEngine;
using UnityEngine.Events;

public class EnemyKamikaze : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] GameObject explosion;
    [SerializeField] float distanceToExplosion = 5;
    [SerializeField] float timeToExplosion = 0.5f;
    [SerializeField] Animator anim;
    [SerializeField] UnityEvent onExplosion;
    bool isExplosionState;
    Transform player;
    EnemyMain enemyMain;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        enemyMain.healtSystem.onDie.AddListener(Explosion);
        enemyMain.speed = enemyMain.speed + GameManager.instance.difficulty * 0.15f;
        timeToExplosion = timeToExplosion - GameManager.instance.difficulty * 0.025f;
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + Mathf.Clamp(GameManager.instance.difficulty - 1,0,Mathf.Infinity) * 0.25f;
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
        enemyMain.SetCurrentHealth(enemyMain.healtSystem.maxHealt);
        if(anim != null)
            anim.speed *= 1 + GameManager.instance.difficulty * 0.25f;
        player = enemyMain.target;
    }

    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(enemyMain == null)
        {
            Destroy(this);
            return;
        }
        if(isExplosionState)
            return;
        if(Vector3.Distance(transform.position,player.position) <= distanceToExplosion)
        {
            isExplosionState = true;
            if(anim != null)
                anim.SetTrigger("Dash");
            onExplosion.Invoke();
            Invoke("TakeDamageAfterTime",timeToExplosion);
            return;
        }
        enemyMain.Move();
        if(anim != null)
            WalkAnimation();
    }
    public void WalkAnimation()
    {
        anim.SetFloat("x",enemyMain.agent.velocity.x);
        anim.SetFloat("y",enemyMain.agent.velocity.z);
    }
    void TakeDamageAfterTime()=>enemyMain.healtSystem.TakeDamage(1488,"LOL");
    void Explosion()
    {
        EZ_PoolManager.Spawn(explosion.transform,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
