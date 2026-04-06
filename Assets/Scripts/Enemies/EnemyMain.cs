using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMain : MonoBehaviour
{
    public enum TypeMovement
    {
        None,
        Move,
        MoveDodge,
        Fly
    }
    [HideInInspector]public HealtSystem healtSystem;
    Rigidbody rb;
    [HideInInspector]public NavMeshAgent agent;
    [HideInInspector]public Transform target;
    Coroutine refreshPathCoroutine;
    Vector3 offsetMove;
    public string nameEnemy;
    public TypeMovement typeMovement;
    public float speed;
    public LayerMask layerAvoidObjects;
    public float randomOffsetDistance = 10;
    public float kdToRandomPath = 0.5f;
    float currentHealth;
    public GameObject damageCounter;
    public GameObject bloodVFX;
    public GameObject explosion;
    Outline outline;
    void Awake()
    {
        TryGetComponent(out agent);
        TryGetComponent(out outline);
        if(agent != null)
            agent.speed = speed;
        TryGetComponent(out rb);
        if(GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
        if(TryGetComponent(out healtSystem))
        {
            SetCurrentHealth(healtSystem.healt);
            healtSystem.onDie.AddListener(() =>
            {
                if(BuffManager.instance.passiveBuff.isExplosionAfterDeath)
                    EZ_PoolManager.Spawn(explosion.transform,transform.position,Quaternion.identity);
                outline.TurnDefualtOutlineColor();
                //ShakeManager.instance.Shake(ShakeManager.ShakeType.Kill);
                ScoreManager.instance.addKill();
                ComboManager.instance.addCombo(1);
                if(typeMovement == TypeMovement.Fly)
                    SetLookAtPlayer();
                Destroy(this);
                if(agent != null)
                    Destroy(agent);
            });
            healtSystem.onTakeDamage.AddListener((Vector3 pos) =>
            {
                EZ_PoolManager.Spawn(bloodVFX.transform,pos,transform.rotation);
                float damage = 0;
                if (healtSystem != null)
                {
                    damage = currentHealth - healtSystem.healt;
                    SetCurrentHealth(healtSystem.healt);
                }
                if (EZ_PoolManager.Instance != null && damageCounter != null)
                {
                    PopupDamage popupDamage = EZ_PoolManager.Spawn(damageCounter.transform, pos + Vector3.up * 0.5f, transform.rotation).GetComponent<PopupDamage>();
                    popupDamage.SetText(Mathf.FloorToInt(Mathf.Abs(damage)) == 0 ? ":)" +
                    "" : $"-{ConvertorValue.FormatFloat(Mathf.Abs(damage))}", currentHealth <= 0 || Mathf.FloorToInt(Mathf.Abs(damage)) == 0);
                }
            });
        }
    }
    public void SetCurrentHealth(float health) => currentHealth = health;
    void Update()
    {
        if(transform.position.y <= -10)
            Destroy(gameObject);
    }
    public void Move()
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(agent != null)
            agent.speed = speed;
        Collider[] avoidObjects = Physics.OverlapSphere(transform.position,3,layerAvoidObjects);
        Vector3 avoidDirection = Vector3.zero;
        if(avoidObjects.Length > 0)
        {
            float currentDistacne = 3;
            for(int i = 0; i < avoidObjects.Length; i++)
            {
                if(avoidObjects[i].transform == transform)
                    continue;
                if(Vector3.Distance(transform.position,avoidObjects[i].transform.position) < currentDistacne)
                {
                    avoidDirection = (transform.position - avoidObjects[i].transform.position).normalized;
                    if(avoidDirection == Vector3.zero)
                    {
                        avoidObjects[i].transform.position += Vector3.right;
                        transform.position -= Vector3.left;
                    }
                    currentDistacne = Vector3.Distance(transform.position,avoidObjects[i].transform.position);
                }
            }
        }
        switch (typeMovement)
        {
            case TypeMovement.MoveDodge:
                if(agent == null)
                    return;
                if(refreshPathCoroutine == null)
                    refreshPathCoroutine = StartCoroutine(PathOffset());
                agent.destination = (avoidDirection != Vector3.zero ? avoidDirection  :  target.position) + offsetMove;
            break;
            case TypeMovement.Move:
                if(agent == null)
                    return;
                agent.destination = target.position;
            break;
            case TypeMovement.Fly:
                SetLookAtPlayer();
                rb.AddForce((avoidDirection != Vector3.zero ? avoidDirection.normalized :  (target.position - transform.position).normalized) * Time.deltaTime * 1000 * speed,ForceMode.Acceleration);
            break;
            case TypeMovement.None:
                if(refreshPathCoroutine != null)
                {
                    StopCoroutine(refreshPathCoroutine);
                    refreshPathCoroutine = null;
                }
            break;
        }
    }
    public void Move(Vector3 customTarget)
    {
        if(Pause.isPaused || GameManager.instance.gameIsStarted == false)
            return;
        if(agent != null)
            agent.speed = speed;
        switch (typeMovement)
        {
            case TypeMovement.Fly:
                SetLookAtPlayer();
                rb.AddForce((customTarget - transform.position).normalized * Time.deltaTime * 1000 * speed,ForceMode.Acceleration);
            break;
            default:
                if(agent == null)
                    return;
                agent.destination = customTarget;
            break;
        }
        if(refreshPathCoroutine != null)
        {
            StopCoroutine(refreshPathCoroutine);
            refreshPathCoroutine = null;
        }
    }
    public void SetLookAtPlayer()
    {
        Quaternion lookAtPlayerRot = Quaternion.LookRotation((target.position - transform.position).normalized);
        lookAtPlayerRot.z = 0;
        lookAtPlayerRot.x = 0;
        transform.rotation = lookAtPlayerRot;
    }
    IEnumerator PathOffset()
    {
        while (true)
        {
            float a = Random.value * (2 * Mathf.PI) - Mathf.PI;
            float x = Mathf.Cos(a);
            float y = Mathf.Sin(a);
            offsetMove = new Vector3(x,0,y) * randomOffsetDistance;
            yield return new WaitForSeconds(kdToRandomPath);
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyMain))]
public class EnemyMainEditor : Editor
{
    SerializedProperty m_nameEnemy;
    SerializedProperty m_typeMovement;
    SerializedProperty m_speed;
    SerializedProperty m_randomOffsetDistance;
    SerializedProperty m_kdToRandomPath;
    SerializedProperty m_layerAvoidObjects;
    SerializedProperty m_bloodVFX;
    SerializedProperty m_explosion;
    SerializedProperty m_damageCounter;
    void OnEnable()
    {
        m_layerAvoidObjects = serializedObject.FindProperty("layerAvoidObjects");
        m_typeMovement = serializedObject.FindProperty("typeMovement");
        m_speed = serializedObject.FindProperty("speed");
        m_randomOffsetDistance = serializedObject.FindProperty("randomOffsetDistance");
        m_kdToRandomPath = serializedObject.FindProperty("kdToRandomPath");
        m_nameEnemy = serializedObject.FindProperty("nameEnemy");
        m_bloodVFX = serializedObject.FindProperty("bloodVFX");
        m_explosion = serializedObject.FindProperty("explosion");
        m_damageCounter = serializedObject.FindProperty("damageCounter");
    }
    public override void OnInspectorGUI()
    {
        EnemyMain enemyMain = (EnemyMain)target;
        serializedObject.Update();
        GUILayout.Label("Values:");
        EditorGUILayout.PropertyField(m_nameEnemy,new GUIContent("Name Enemy"));
        EditorGUILayout.PropertyField(m_typeMovement,new GUIContent("Type Movement"));
        EditorGUILayout.PropertyField(m_layerAvoidObjects, new GUIContent("Layer Avoid Objects"));
        EditorGUILayout.PropertyField(m_speed, new GUIContent("Speed"));
        switch (enemyMain.typeMovement)
        {
            case EnemyMain.TypeMovement.MoveDodge:
                EditorGUILayout.PropertyField(m_randomOffsetDistance, new GUIContent("Random Offset Distance"));
                EditorGUILayout.PropertyField(m_kdToRandomPath, new GUIContent("Kd To Random Path"));
            break;

        }
        GUILayout.Label("Damage:");
        EditorGUILayout.PropertyField(m_bloodVFX, new GUIContent("Blood VFX"));
        EditorGUILayout.PropertyField(m_explosion, new GUIContent("Explosion"));
        EditorGUILayout.PropertyField(m_damageCounter, new GUIContent("Damage Counter"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif