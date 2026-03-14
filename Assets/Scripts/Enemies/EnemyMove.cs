using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    EnemyMain enemyMain;
    void Start()=>enemyMain = GetComponent<EnemyMain>();
    void Update()
    {
        if(enemyMain == null)
        {
            Destroy(this);
            return;
        }
        enemyMain.Move();
    }
}
