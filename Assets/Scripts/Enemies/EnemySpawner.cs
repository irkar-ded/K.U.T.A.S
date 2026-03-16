
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
    public bool useHealthbar;
    public EnemyMain SpawnEnemy()
    {
        System.Array.Sort(enemyContents,(a,b) => a.startInStage.CompareTo(b.startInStage));
        List<EnemyContent> enemiesContentTemp = new List<EnemyContent>(enemyContents);
        enemiesContentTemp.RemoveAll(x => x.startInStage > GameManager.instance.stage);
        while(enemiesContentTemp.Count > 0)
        {
            EnemyContent enemyContentTemp = enemiesContentTemp[Random.Range(0,enemiesContentTemp.Count)];
            if(Random.Range(0,101) <= enemyContentTemp.chance)
                return Instantiate(enemyContentTemp.enemy,transform.position,transform.rotation);
            enemiesContentTemp.Remove(enemyContentTemp);
        }
        return null;
    }
}
