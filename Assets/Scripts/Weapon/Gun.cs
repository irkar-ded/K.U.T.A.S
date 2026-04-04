using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public enum OwnerGun
    {
        Player,
        Enemy
    }
    [Header("Values:")]
    public OwnerGun ownerGun;
    public LayerMask surfaceToLook;
    public LayerMask mouseToLook;
    [Header("Bullet:")]
    public GameObject bullet;
    public Transform bulletSpawnPosition;
    public Bullet.ParametersBullet parametersBullet;
    [Header("Gun:")]
    public EventReference soundShoot;
    public Transform mainGun;
    public Animator gunAnim;
    public float recoil = 0;
    public float kdBeetwenShoots = 0.1f;
    public static Gun instance;
    Controls gameInputs;
    InputAction shootKey;
    float timerKd;
    Transform player;
    void Awake()
    {
        if(ownerGun == OwnerGun.Enemy)
        {
            player = GetComponentInParent<EnemyMain>().target;
            timerKd = Random.Range(0,kdBeetwenShoots);
        }
        else
        {
            instance = this;
            if (SettingsManager.instance != null)
                gameInputs = SettingsManager.gameInputs;
            else
                gameInputs = new Controls();
            shootKey = gameInputs.Player.Fire;
            shootKey.Enable();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        RotateGun();
        Reload();
        if(ownerGun != OwnerGun.Player)
            return;
        if(shootKey.IsPressed())
            Shoot();
    }
    void OnDisable()
    {
        if(ownerGun == OwnerGun.Player)
            shootKey.Disable();
    }
    public void RotateGun() => mainGun.rotation = LookRotate();
    public void Reload()
    {
        if(timerKd > 0)
            timerKd -=Time.deltaTime;
    }
    public void Shoot()
    {
        if(timerKd > 0)
            return;
        if(ownerGun == OwnerGun.Player)
        {
            Hitmark.instance.PlayHitmarkAnim(false);
            //ShakeManager.instance.Shake(ShakeManager.ShakeType.Fire);
        }
        if(gunAnim != null)
            gunAnim.SetTrigger("Shoot");
        RuntimeManager.PlayOneShot(soundShoot,transform.position);
        SpawnBullet();
        timerKd = kdBeetwenShoots;
        //Debug.LogError("LOL");
    }
    public Vector3 getTargetLook() => ownerGun == OwnerGun.Player ? MouseHitWhithLayer() : player.position;
    public void SpawnBullet()=>EZ_PoolManager.Spawn(bullet.transform, bulletSpawnPosition.position + bulletSpawnPosition.right * Random.Range(-recoil,recoil), bulletSpawnPosition.rotation).GetComponent<Bullet>().SetBullet(parametersBullet); 
    public Quaternion LookRotate()
    {
        Quaternion rotToLook = Quaternion.LookRotation(getTargetLook() - mainGun.position).normalized;
        rotToLook.z = 0;
        rotToLook.x = 0;
        return rotToLook;
    }
    public Vector3 MouseHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity, surfaceToLook))
            return new Vector3(hit.point.x,transform.position.y,hit.point.z);
        return Vector3.zero;
    }
    public Vector3 MouseHitWhithLayer()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity, mouseToLook))
            return new Vector3(hit.point.x,transform.position.y,hit.point.z);
        return Vector3.zero;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{
    SerializedProperty m_ownerGun;
    SerializedProperty m_parametersBullet;
    SerializedProperty m_bullet;
    SerializedProperty m_surfaceToLook;
    SerializedProperty m_kdBeetwenShoots;
    SerializedProperty m_mouseToLook;
    SerializedProperty m_bulletSpawnPosition;
    SerializedProperty m_soundShoot;
    SerializedProperty m_mainGun;
    SerializedProperty m_gunAnim;
    SerializedProperty m_recoil;
    void OnEnable()
    {
        m_ownerGun = serializedObject.FindProperty("ownerGun");
        m_surfaceToLook = serializedObject.FindProperty("surfaceToLook");
        m_parametersBullet = serializedObject.FindProperty("parametersBullet");
        m_bullet = serializedObject.FindProperty("bullet");
        m_kdBeetwenShoots = serializedObject.FindProperty("kdBeetwenShoots");
        m_mouseToLook = serializedObject.FindProperty("mouseToLook");
        m_bulletSpawnPosition = serializedObject.FindProperty("bulletSpawnPosition");
        m_soundShoot = serializedObject.FindProperty("soundShoot");
        m_mainGun = serializedObject.FindProperty("mainGun");
        m_gunAnim = serializedObject.FindProperty("gunAnim");
        m_recoil = serializedObject.FindProperty("recoil");
    }
    public override void OnInspectorGUI()
    {
        Gun gun = (Gun)target;
        serializedObject.Update();
        GUILayout.Label("Values:");
        EditorGUILayout.PropertyField(m_ownerGun, new GUIContent("Owner Gun"));
        EditorGUILayout.PropertyField(m_surfaceToLook,new GUIContent("Surface To Look"));
        if(gun.ownerGun == Gun.OwnerGun.Player)
            EditorGUILayout.PropertyField(m_mouseToLook,new GUIContent("Mouse To Look"));
        GUILayout.Label("Bullet:");
        EditorGUILayout.PropertyField(m_bullet,new GUIContent("Bullet Prefab"));
        EditorGUILayout.PropertyField(m_bulletSpawnPosition,new GUIContent("Bullet Spawn Position"));
        EditorGUILayout.PropertyField(m_parametersBullet,new GUIContent("Bullet Parameters"));
        GUILayout.Label("Gun:");
        EditorGUILayout.PropertyField(m_soundShoot,new GUIContent("Sound Shoot"));
        EditorGUILayout.PropertyField(m_mainGun,new GUIContent("Main Gun"));
        EditorGUILayout.PropertyField(m_gunAnim,new GUIContent("Gun Anim"));
        EditorGUILayout.PropertyField(m_kdBeetwenShoots,new GUIContent("Kd Beetwen Shoots"));
        EditorGUILayout.PropertyField(m_recoil,new GUIContent("Recoil"));
        serializedObject.ApplyModifiedProperties();
    }
#endif
}