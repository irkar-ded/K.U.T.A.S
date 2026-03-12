using System.Collections;
using System.Collections.Generic;
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
    [Header("Bullet:")]
    public GameObject bullet;
    public Bullet.ParametersBullet parametersBullet;
    [Header("Gun:")]
    public float kdBeetwenShoots = 0.1f;
    float timerKd;
    Transform player;
    void Start()
    {
        if(ownerGun == OwnerGun.Enemy)
        {
            player = GetComponentInParent<EnemyMain>().target;
            timerKd = Random.Range(0,kdBeetwenShoots);
        }
    }
    // Update is called once per frame
    void Update()
    {
        RotateGun();
        Reload();
        if(ownerGun != OwnerGun.Player)
            return;
        if(Mouse.current.leftButton.isPressed)
            Shoot();
    }
    public void RotateGun() => transform.rotation = LookRotate();
    public void Reload()
    {
        if(timerKd > 0)
            timerKd -=Time.deltaTime;
    }
    public void Shoot()
    {
        if(timerKd > 0)
            return;
        SpawnBullet();
        timerKd = kdBeetwenShoots;
    }
    public void SpawnBullet()=>Instantiate(bullet, transform.position, transform.rotation).GetComponent<Bullet>().SetBullet(parametersBullet); 
    public Quaternion LookRotate()
    {
        Quaternion rotToLook = Quaternion.LookRotation(((ownerGun == OwnerGun.Player ? MouseHit() : player.position) - transform.position).normalized);
        rotToLook.z = 0;
        rotToLook.x = 0;
        return rotToLook;
    }
    public Vector3 MouseHit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit,Mathf.Infinity, surfaceToLook))
            return hit.point;
        return Vector3.zero;
    }
}
/*[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{
    SerializedProperty m_ownerGun;
    SerializedProperty m_parametersBullet;
    SerializedProperty m_bullet;
    SerializedProperty m_surfaceToLook;
    SerializedProperty m_kdBeetwenShoots;
    SerializedProperty m_reactionTime;
    void OnEnable()
    {
        m_ownerGun = serializedObject.FindProperty("ownerGun");
        m_surfaceToLook = serializedObject.FindProperty("surfaceToLook");
        m_parametersBullet = serializedObject.FindProperty("parametersBullet");
        m_bullet = serializedObject.FindProperty("bullet");
        m_kdBeetwenShoots = serializedObject.FindProperty("kdBeetwenShoots");
        m_reactionTime = serializedObject.FindProperty("reactionTime");
    }
    public override void OnInspectorGUI()
    {
        Gun gun = (Gun)target;
        serializedObject.Update();
        GUILayout.Label("Values:");
        EditorGUILayout.PropertyField(m_ownerGun, new GUIContent("Owner Gun"));
        EditorGUILayout.PropertyField(m_surfaceToLook,new GUIContent("Surface To Look"));
        GUILayout.Label("Bullet:");
        EditorGUILayout.PropertyField(m_bullet,new GUIContent("Bullet Prefab"));
        EditorGUILayout.PropertyField(m_parametersBullet,new GUIContent("Bullet Parameters"));
        GUILayout.Label("Gun:");
        EditorGUILayout.PropertyField(m_kdBeetwenShoots,new GUIContent("Kd Beetwen Shoots"));
        serializedObject.ApplyModifiedProperties();
    }
}*/