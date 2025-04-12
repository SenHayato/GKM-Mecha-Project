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
    public LayerMask hitLayer;
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
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentWaypoints = -1;

        GoToNexPoint();

    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        // Patrolling
        if (agent.remainingDistance < 0.5f)
        {
            if (model.maxWaitingTime == 0)
                model.maxWaitingTime = 5f;

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
        if (wayPoints.Length != 0 )
        {
            currentWaypoints = (currentWaypoints + 1) % wayPoints.Length;
            agent.SetDestination(wayPoints[currentWaypoints].position);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void AttackRanged()
    {
        model.isAttacking = true;
        if(model.enemyType == EnemyType.EnemyRange)
        {
            Vector3 targetPoint;
            Vector3 attackOrigin = transform.position + Vector3.up * 1.5f;
            Vector3 directionToPlayer = (playerTransform.position + Vector3.up * 1.0f - attackOrigin).normalized;
            RaycastHit hit;

            Debug.DrawRay(attackOrigin, 3 * model.attackRange * directionToPlayer, Color.magenta, 1.0f);

           if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, model.attackRange * 8, hitLayer)){
                targetPoint = hit.point;

                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.gameObject.TryGetComponent<PlayerActive>(out var playerActive);
                    if(playerActive != null)
                    {
                        playerActive.TakeDamage(model.attackPower);
                        Debug.Log($"Range attack hit player for {model.attackPower} damage");
                    }
                    else
                    {
                        Debug.LogWarning("Player hit but no PlayerHealth component found");
                    }
                }
            }
            else
            {
                //targetPoint = attackOrigin + (directionToPlayer * model.attackRange * 3);
            }
           //StartCoroutine(ResetAttack());
        }
    }

    void ResetAttack()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.detectionRange);
    }
}
