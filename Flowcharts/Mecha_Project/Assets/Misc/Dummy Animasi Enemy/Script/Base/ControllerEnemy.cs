using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    private Transform[] wayPoints;
    private int currentWaypoints;

    private bool playerInSight = false;
    private bool playerInFieldOfView = false;
    private bool isObstacleInTheWay = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        model = GetComponent<EnemyModel>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentWaypoints = -1;

        GoToNexPoint();

    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        // Patrolling
        if (agent.remainingDistance > 0.5f)
        {
            if (model.maxWaitingTime == 0)
                model.maxWaitingTime = Random.Range(2, 6);

            if (model.patrolWaitTime >= model.maxWaitingTime)
            {
                model.maxWaitingTime = 0;
                model.patrolWaitTime = 0;
                GoToNexPoint();
            }
            else
            {
                model.patrolWaitTime += Time.deltaTime;
            }
        }
        // Memaksa Enemy untuk menghadap player jika dalam keadaan mengejar
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
    
    void GoToNexPoint()
    {
        if (wayPoints.Length == 0 )
        {
            return;
        }
        float distanceToWayPoint = Vector3.Distance(wayPoints[currentWaypoints].position, transform.position);
        if (distanceToWayPoint <= 3)
        {
            currentWaypoints = (currentWaypoints + 1) % wayPoints.Length;
            agent.SetDestination(wayPoints[currentWaypoints].position);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Patrol()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.detectionRange);
    }
}
