using System.Collections;
using System.Collections.Generic;
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
    [Header("Values")]
    public float speed;
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
    public void Move(TypeMovement typeMovement)
    {
        if(agent != null)
            agent.speed = speed;
        switch (typeMovement)
        {
            case TypeMovement.MoveDodge:
                if(refreshPathCoroutine == null)
                    refreshPathCoroutine = StartCoroutine(PathOffset(TypeMovement.MoveDodge));
                agent.destination = target.position + offsetMove;
            break;
            case TypeMovement.Move:
                agent.destination = target.position;
            break;
            case TypeMovement.Fly:
                rb.AddForce((target.position - transform.position).normalized * speed,ForceMode.Acceleration);
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
    IEnumerator PathOffset(TypeMovement typeMovement)
    {
        while (true)
        {
            switch (typeMovement)
            {
                case TypeMovement.MoveDodge:
                    float a = Random.value * (2 * Mathf.PI) - Mathf.PI;
                    float x = Mathf.Cos(a);
                    float y = Mathf.Sin(a);
                    offsetMove = new Vector3(x,0,y) * 10;
                break;
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
