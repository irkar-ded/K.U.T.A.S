using System.Collections;
using System.Collections.Generic;
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
    HealtSystem healtSystem;
    Rigidbody rb;
    NavMeshAgent agent;
    [HideInInspector]public Transform target;
    Coroutine refreshPathCoroutine;
    Vector3 offsetMove;
    public TypeMovement typeMovement;
    public float speed;
    public LayerMask layerAvoidObjects;
    public float randomOffsetDistance = 10;
    public float kdToRandomPath = 0.5f;
    void Awake()
    {
        TryGetComponent(out agent);
        if(agent != null)
            agent.speed = speed;
        TryGetComponent(out rb);
        if(GameObject.FindWithTag("Player"))
            target = GameObject.FindWithTag("Player").transform;
        if(TryGetComponent(out healtSystem))
        {
            healtSystem.onDie.AddListener(() =>
            {
                Destroy(this);
                if(agent != null)
                    Destroy(agent);
            });
        }
    }
    public void Move()
    {
        if(agent != null)
            agent.speed = speed;
        switch (typeMovement)
        {
            case TypeMovement.MoveDodge:
                if(refreshPathCoroutine == null)
                    refreshPathCoroutine = StartCoroutine(PathOffset());
                agent.destination = target.position + offsetMove;
            break;
            case TypeMovement.Move:
                agent.destination = target.position;
            break;
            case TypeMovement.Fly:
                FlyMovement();
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
        if(agent != null)
            agent.speed = speed;
        switch (typeMovement)
        {
            case TypeMovement.Fly:
                rb.AddForce((customTarget - transform.position).normalized * Time.deltaTime * 1000 * speed,ForceMode.Acceleration);
            break;
            default:
                agent.destination = customTarget;
            break;
        }
        if(refreshPathCoroutine != null)
        {
            StopCoroutine(refreshPathCoroutine);
            refreshPathCoroutine = null;
        }
    }
    public void FlyMovement()
    {
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
        rb.AddForce((avoidDirection != Vector3.zero ? avoidDirection :  (target.position - transform.position).normalized)  * speed,ForceMode.Acceleration);
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
    SerializedProperty m_typeMovement;
    SerializedProperty m_speed;
    SerializedProperty m_randomOffsetDistance;
    SerializedProperty m_kdToRandomPath;
    SerializedProperty m_layerAvoidObjects;
    void OnEnable()
    {
        m_layerAvoidObjects = serializedObject.FindProperty("layerAvoidObjects");
        m_typeMovement = serializedObject.FindProperty("typeMovement");
        m_speed = serializedObject.FindProperty("speed");
        m_randomOffsetDistance = serializedObject.FindProperty("randomOffsetDistance");
        m_kdToRandomPath = serializedObject.FindProperty("kdToRandomPath");
    }
    public override void OnInspectorGUI()
    {
        EnemyMain enemyMain = (EnemyMain)target;
        serializedObject.Update();
        GUILayout.Label("Values:");
        EditorGUILayout.PropertyField(m_typeMovement,new GUIContent("Type Movement"));
        EditorGUILayout.PropertyField(m_speed, new GUIContent("Speed"));
        switch (enemyMain.typeMovement)
        {
            case EnemyMain.TypeMovement.MoveDodge:
                EditorGUILayout.PropertyField(m_randomOffsetDistance, new GUIContent("Random Offset Distance"));
                EditorGUILayout.PropertyField(m_kdToRandomPath, new GUIContent("Kd To Random Path"));
            break;
            case EnemyMain.TypeMovement.Fly:
                EditorGUILayout.PropertyField(m_layerAvoidObjects, new GUIContent("Layer Avoid Objects"));
            break;

        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif