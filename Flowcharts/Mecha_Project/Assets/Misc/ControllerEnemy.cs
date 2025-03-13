using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControllerEnemy : MonoBehaviour
{
    [Header("Layering")]
    public LayerMask hitLayer;
    public LayerMask obstacle;
    private EnemyActive enemyActive;
    private EnemyModel enemyModel;
    
    public NavMeshAgent navAgent;
    private bool isWaiting;


    public Transform centrePoint; // Area central yang membuat enemy dapat bergerak bebas didalamnya
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        if (enemyModel != null)
        {
            enemyModel.startPosition = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWaiting && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            StartCoroutine(CooldownPatrol());
        }
    }
    private IEnumerator CooldownPatrol()
    {
        isWaiting = true;
        yield return new WaitForSeconds(enemyModel.patrolWaitTime);
        PatrolRange();
        isWaiting = false;
    }
    private void PatrolRange()
    {
        if (navAgent == null || !navAgent.enabled) return;

        Vector3 randomDirection = Random.insideUnitSphere * enemyModel.patrolRadius;
        randomDirection.y = 0;
        Vector3 targetPosition = enemyModel.startPosition + randomDirection;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(targetPosition, out hit, enemyModel.patrolRadius, NavMesh.AllAreas))
        {
            enemyModel.currentDestination = hit.position;
            navAgent.SetDestination(enemyModel.currentDestination);
        }
    }

    void FieldOfView()
    {

    }
}
