using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllerEnemy : MonoBehaviour
{
    // State Management
    private enum AIState { Idle, Patrol, Chase, Attack}
    private AIState currentState = AIState.Idle;

    // Components
    private NavMeshAgent agent;
    private Transform playerTransform;
    private EnemyModel model;
    private EnemyActive active;
    private Vector3 target;

    private bool playerInSight = false;
    private bool playerInFieldOfView = false;
    private bool isObstacleInTheWay = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        model = GetComponent<EnemyModel>();
        target = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;


    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= model.detectionRange)
        {
            agent.SetDestination(playerTransform.position);

            if(distance <= agent.stoppingDistance)
            {
                // Attack the Target
                // Face Target
                FaceTarget();
            }
        }
        // Update timers
        if (model.attackTimer > 0)
            model.attackTimer -= Time.deltaTime;
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.detectionRange);
    }
}
