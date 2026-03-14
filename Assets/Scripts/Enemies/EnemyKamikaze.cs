using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKamikaze : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float distanceToExplosion = 5;
    [SerializeField] float timeToExplosion = 0.5f;
    HealtSystem healtSystem;
    bool isExplosionState;
    Transform player;
    EnemyMain enemyMain;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        healtSystem = GetComponent<HealtSystem>();
        healtSystem.onDie.AddListener(Explosion);
        player = enemyMain.target;
    }

    // Update is called once per frame
    void Update()
    {
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
            Invoke("TakeDamageAfterTime",timeToExplosion);
            return;
        }
        enemyMain.Move();
    }
    void TakeDamageAfterTime()=>healtSystem.TakeDamage(1488);
    void Explosion()
    {
        Instantiate(explosion,transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
