using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [System.Serializable]
    public class ParametersBullet
    {
        public GameObject owner;
        public float force;
        public float damage;
    }
    [SerializeField] LayerMask layerWall;
    [SerializeField] float radiusWallCollision = 0.01f;
    Rigidbody rb;
    DamageOnCollision damage;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        damage = GetComponentInChildren<DamageOnCollision>();
        damage.OnTakeDamage.AddListener(() => gameObject.SetActive(false));
    }
    void FixedUpdate()
    {
        if(Physics.CheckSphere(transform.position,radiusWallCollision,layerWall))
            gameObject.SetActive(false);
    }
    void OnDrawGizmosSelected()
    {
        Color colorCollision = Color.green;;
        colorCollision.a = 0.5f;
        Gizmos.color = colorCollision;
        Gizmos.DrawSphere(transform.position,radiusWallCollision);
    }
    void OnEnable()=>damage.enabled = false;
    public void SetBullet(ParametersBullet bullet)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * bullet.force,ForceMode.Impulse);
        damage.owner = bullet.owner;
        damage.damage = bullet.damage;
        damage.enabled = true;
    }
}
