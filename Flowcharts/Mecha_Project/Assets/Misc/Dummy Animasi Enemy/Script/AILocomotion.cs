using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    private enum AIState { Idle, Patrol, Chase, Attack, Dead }
    private AIState currentState = AIState.Idle;
    private AIState previousState;

    [Header("Komponen Enemy")]
    public Transform mechaPlayer;
    public Transform lookTarget;
    private NavMeshAgent agent;

    [Header("Script")]
    private EnemyModel model;
    private EnemyActive active;

    [Header("Komponen Lainnya")]
    public Transform[] wayPoint;
    private int _currentWayPoint = 0;
    private float _speed = 2f;

    private void Awake()
    {
        model = GetComponent<EnemyModel>();
        active = GetComponent<EnemyActive>();
        agent = GetComponent<NavMeshAgent>();
        mechaPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPoint();
        if (agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitTime());                    
        }
    }

    void ApplyMovement()
    {

    }
    void GoToNextPoint()
    {
        //Kmbali jika tidak ada waypoint yang di pasang
        if (wayPoint.Length == 0) return;
        agent.destination = wayPoint[_currentWayPoint].position;
        _currentWayPoint = (_currentWayPoint + 1) % wayPoint.Length;

    }
    IEnumerator WaitTime()
    {
        if(currentState == AIState.Patrol)
        {
            
        }
        yield return new WaitForSeconds(model.patrolWaitTime);
    }
    void Chase()
    {

    }
    void AttackRanged()
    {

    }

    void AttackShort()
    {

    }

    void ResetAttack()
    {

    }

    

}
