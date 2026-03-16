using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public class TeleportBulletSettings
    {
        public GameObject teleporter;
        public Transform teleportTo;
        public Vector2 savePosition;
        public TeleportBulletSettings(GameObject teleporter,Transform teleportTo,Vector2 savePosition)
        {
            this.teleporter = teleporter;
            this.teleportTo = teleportTo;
            this.savePosition = savePosition;
        }
    }
    [System.Serializable]
    public class ParametersBullet
    {
        [Header("Values:")]
        public GameObject owner;
        public float force;
        public float damage;
        [Header("Buffs:")]
        public int toxicBullet;
        public int xRayBullet;
        public int bounceBullet;
    }
    [Header("Values:")]
    [SerializeField] LayerMask layerWall;
    [SerializeField] float radiusWallCollision = 0.01f;
    [Header("Buffs")]
    [SerializeField] Color toxicColor = Color.green;
    [SerializeField] Color xRayColor = Color.magenta;
    [SerializeField] Color bounceColor = Color.cyan;
    List<TeleportBulletSettings> teleportBulletSettings = new List<TeleportBulletSettings>();
    int canGoOutWall;
    int canBounceWall;
    Color startColor;
    ParametersBullet currentParameter;
    MeshRenderer model;
    Rigidbody rb;
    DamageOnCollision damage;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        model = GetComponent<MeshRenderer>();
        startColor = model.material.color;
        damage = GetComponentInChildren<DamageOnCollision>();
        damage.OnTakeDamage.AddListener(OnTakeDamage);
        SetupTeleportBulletSettings();
    }
    public void SetupTeleportBulletSettings()
    {
        teleportBulletSettings.Add(new TeleportBulletSettings(GameObject.Find("TeleportBulletLeft"),GameObject.Find("TeleportBulletPointRight").transform,Vector2.up));
        teleportBulletSettings.Add(new TeleportBulletSettings(GameObject.Find("TeleportBulletRight"),GameObject.Find("TeleportBulletPointLeft").transform,Vector2.up));
        teleportBulletSettings.Add(new TeleportBulletSettings(GameObject.Find("TeleportBulletBackward"),GameObject.Find("TeleportBulletPointForward").transform,Vector2.right));
        teleportBulletSettings.Add(new TeleportBulletSettings(GameObject.Find("TeleportBulletForward"),GameObject.Find("TeleportBulletPointBackward").transform,Vector2.right));
    }
    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusWallCollision, layerWall);
        if (colliders.Length > 0)
        {
            if(canBounceWall > 0)
                BounceBullet();
            else if(canGoOutWall > 0)
            {
                if(colliders[0].tag == "TeleportBullet")
                {
                    print(colliders[0].name);
                    TeleportBullet(colliders[0].gameObject.name);
                }
                else
                    canGoOutWall--;
            }
            else
                gameObject.SetActive(false);
        }
    }
    public void BounceBullet()
    {
        Physics.Raycast(transform.position,Vector3.forward,out RaycastHit hit,1,layerWall);
        rb.velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation((Vector3.Reflect(rb.velocity.normalized,hit.normal)- transform.position).normalized);
        rb.AddForce(transform.forward * currentParameter.force,ForceMode.Impulse);
        canBounceWall--;
    }
    public void TeleportBullet(string nameTeleporter)
    {
        TeleportBulletSettings teleportBulletSetup = teleportBulletSettings.Find(x=>x.teleporter.name == nameTeleporter);
        Vector3 posToTeleport = teleportBulletSetup.teleportTo.position;
        if(teleportBulletSetup.savePosition.x > 0)
            posToTeleport.x = transform.position.x;
        if(teleportBulletSetup.savePosition.y > 0)
            posToTeleport.z = transform.position.z;
        transform.position = posToTeleport;
    }
    void OnTakeDamage()
    {
        if(currentParameter.toxicBullet > 0)
            damage.lastDamagedTarget.DamagePoison(currentParameter.toxicBullet,currentParameter.damage / 20);
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
        currentParameter = bullet;
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.forward * currentParameter.force,ForceMode.Impulse);
        damage.owner = currentParameter.owner;
        damage.damage = currentParameter.damage;
        damage.enabled = true;
        canBounceWall = currentParameter.bounceBullet;
        if(currentParameter.xRayBullet > 0)
            canGoOutWall = currentParameter.xRayBullet + (currentParameter.xRayBullet * 2) + 1;
        SetColorBullet();
    }
    public void SetColorBullet()
    {
        Color finalColor = startColor;
        if(currentParameter.toxicBullet > 0)
            finalColor *= toxicColor;
        if(currentParameter.xRayBullet > 0)
            finalColor *= xRayColor;
        if(currentParameter.bounceBullet > 0)
            finalColor *= bounceColor;
        model.material.SetColor("_Color",finalColor);
    }
}
