using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKamikaze : MonoBehaviour
{
    EnemyMain enemyMain;
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
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
        enemyMain.Move(EnemyMain.TypeMovement.Move);
    }
}
