using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using FMODUnity;
using UnityEngine;

public class FinalBoss : MonoBehaviour
{
    [Header("Shoot State:")]
    [SerializeField] EventReference soundShoot;
    [SerializeField] GameObject bullet;
    [SerializeField] Bullet.ParametersBullet parametersBullet;
    [Header("Teto:")]
    [SerializeField] EventReference tetoSound;
    [SerializeField] GameObject teto;
    Interactable interactable;
    GameObject credits;
    EnemyMain enemyMain;
    // Start is called before the first frame update
    void Start()
    {
        enemyMain = GetComponent<EnemyMain>();
        interactable = GetComponent<Interactable>();
        credits = FindObjectOfType<Credits>(true).transform.parent.gameObject;
        credits.SetActive(false);
        Bullet.ParametersBullet parametersPlayer = GameManager.instance.currentPlayer.GetComponentInChildren<Gun>().parametersBullet;
        parametersBullet.owner = gameObject;
        parametersBullet.damage = parametersPlayer.damage;
        parametersBullet.force = parametersPlayer.force;
        parametersBullet.toxicBullet = parametersPlayer.toxicBullet;
        parametersBullet.xRayBullet = parametersPlayer.xRayBullet;
        parametersBullet.bounceBullet = parametersPlayer.bounceBullet;
        enemyMain.healtSystem.onTakeDamage.AddListener((Vector3) => Shoot());
        enemyMain.healtSystem.onDie.AddListener(() => Destroy(gameObject));
        interactable.onUse.AddListener(() =>
        {
            credits.SetActive(true);
            RuntimeManager.PlayOneShot(tetoSound,transform.position);
            Instantiate(teto,transform.position,Quaternion.identity);
        });
        enemyMain.healtSystem.maxHealt = enemyMain.healtSystem.maxHealt + (GameManager.instance.difficulty * 67)  + (BuffManager.instance.passiveBuff.maxHealth * 10);
        enemyMain.healtSystem.healt = enemyMain.healtSystem.maxHealt;
        enemyMain.SetCurrentHealth(enemyMain.healtSystem.maxHealt);
    }
    void Shoot()
    {
        RuntimeManager.PlayOneShot(soundShoot,transform.position);
        EZ_PoolManager.Spawn(bullet.transform, transform.position, Quaternion.LookRotation((enemyMain.target.position - transform.position).normalized)).GetComponent<Bullet>().SetBullet(parametersBullet);
    }
}
