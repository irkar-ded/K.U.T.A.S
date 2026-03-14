
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyContent
    {
        public EnemyMain enemy;
        public float chance = 100;
        public int startInStage = 0;
    }
    [SerializeField] EnemyContent[] enemyContents;
    public EnemyMain SpawnEnemy()
    {
        System.Array.Sort(enemyContents,(a,b) => a.startInStage.CompareTo(b.startInStage));
        for(int i = 0;i < enemyContents.Length; i++)
        {
            if(enemyContents[i].startInStage <= GameManager.instance.stage && Random.Range(0,101) <= enemyContents[i].chance)
                return Instantiate(enemyContents[i].enemy,transform.position,transform.rotation);
        }
        return null;
    }
}
