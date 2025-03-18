using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControllerEnemy : MonoBehaviour
{
    public LayerMask hitLayer;
    public LayerMask obstacleLayer; //Ngeblock vision
    private EnemyModel enemyModel;
    private EnemyActive enemyActive;
    private NavMeshAgent navAgent;
    public LineRenderer lineOfSight;
    private Mesh fovMesh; //Visualisasi Fov


    // State management
    private enum AIState { Idle, Patrol, Chase, Attack, Retreat, Dead }
    private AIState currentState = AIState.Idle;
    private AIState previousState;

    private Transform playerTransform;
    private bool playerInSight = false;
    private bool playerInFieldOfView = false;
    private bool isObstacleInTheWay = false;

    private void Awake()
    {
        enemyModel = GetComponent<EnemyModel>();
        navAgent = GetComponent<NavMeshAgent>();

        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Default", "Environment", "Wall");
        }
    }

    private void Update()
    {
        if (enemyModel.isDeath) return;


    }

    private void ChaseState()
    {
        if (navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(playerTransform.position);

            // Pengecekan apakah code bekerja
            Debug.DrawLine(transform.position, playerTransform.position, Color.blue, 0.2f);
        }

        // Cek apakah berada di range attack
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float effectiveAttackRange = (enemyModel.enemyType == EnemyType.EnemyRange) ? enemyModel.attackRange * 2.5f : enemyModel.attackRange;

        if (distanceToPlayer <= effectiveAttackRange)
        {
            currentState = AIState.Attack;
            Debug.Log("Switching to Attack State : distance" + distanceToPlayer + ", attakc range = " + effectiveAttackRange);
        }
    }
    private void IdleState()
    {
        if (playerInSight && !isObstacleInTheWay)
        {
            currentState = AIState.Chase;
            return;
        }

        //if(Random.value < 0.3f)
        {
            currentState = AIState.Patrol;
            //GeneratePatrolDestination();
        }
    }
    private void PatrolState()
    {
        // Jika Player ditemukan, maka kejar dia

        // Check jika kita membutuhkan destinasi patrol baru
        if (enemyModel.destinationChangeTimer <= 0f || (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f))
        {
            enemyModel.destinationChangeTimer = enemyModel.patrolWaitTime;

            // Antara ke idle atau mencari rute baru untuk point patrol
            //if (Random.value < 0.3f)
            {
                currentState = AIState.Idle;
                if (navAgent != null)
                {
                    navAgent.SetDestination(transform.position);
                }
                //}
                //else
                //{
                //    GeneratePatrolDestination();
                //}
            }
        }
       
    }
    private void GeneratePatrolDestination()
    {
        if (navAgent == null || !navAgent.enabled)
            return;

        // Men Generate patrol secara acak di dalam radius
        //Vector3 randomDirection = Random.insideUnitSphere * enemyModel.patrolRadius;
        //randomDirection.y = 0; // Melihat target pada sumbu Y yang sama
        // Vector3 targetPosition = enemyModel.startPosition + randomDirection;

        NavMeshHit navHit;
        //if (NavMesh.SamplePosition(targetPosition, out navHit, enemyModel.patrolRadius, NavMesh.AllAreas))
        //{
        //    enemyModel.currentDestination = navHit.position;
        //    navAgent.SetDestination(enemyModel.currentDestination);
        //}
    }
}
