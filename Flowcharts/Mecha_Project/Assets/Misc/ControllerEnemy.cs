using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ControllerEnemy : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float viewAngle = 90f;
    public float patrolRadius = 10f;
    public LayerMask obstacleLayer;

    // Components
    private NavMeshAgent navAgent;
    private Transform playerTransform;

    // State management
    private enum AIState { Idle, Patrol, Chase, Attack }
    private AIState currentState = AIState.Idle;

    // Detection flags
    private bool playerVisible = false;
    private Vector3 startPosition;
    private float attackCooldown = 2f;
    private float attackTimer = 0f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Start AI behavior
        StartCoroutine(UpdateAI());
    }

    void Update()
    {
        // Update timers
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // Check if player is visible
        CheckPlayerVisibility();

        // Debug visualization
        DrawDebugVisuals();
    }

    // Main AI state machine
    private IEnumerator UpdateAI()
    {
        while (true)
        {
            switch (currentState)
            {
                case AIState.Idle:
                    // Randomly start patrolling
                    //if (Random.value < 0.3f)
                    {
                        currentState = AIState.Patrol;
                        GeneratePatrolDestination();
                    }

                    // If player spotted, chase
                    if (playerVisible)
                    {
                        currentState = AIState.Chase;
                    }
                    break;

                case AIState.Patrol:
                    // If player spotted, chase
                    if (playerVisible)
                    {
                        currentState = AIState.Chase;
                    }

                    // If reached destination, go back to idle
                    if (navAgent.remainingDistance < 0.5f)
                    {
                        currentState = AIState.Idle;
                    }
                    break;

                case AIState.Chase:
                    // If player not visible, go back to patrol
                    if (!playerVisible)
                    {
                        StartCoroutine(SearchLastKnownPosition());
                        break;
                    }

                    // Move toward player
                    navAgent.SetDestination(playerTransform.position);

                    // If in attack range, attack
                    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                    if (distanceToPlayer <= attackRange)
                    {
                        currentState = AIState.Attack;
                    }
                    break;

                case AIState.Attack:
                    // If player not visible or out of range, chase
                    if (!playerVisible || Vector3.Distance(transform.position, playerTransform.position) > attackRange)
                    {
                        currentState = AIState.Chase;
                        break;
                    }

                    // Face the player
                    LookAtPlayer();

                    // Stop moving
                    navAgent.isStopped = true;

                    // Attack if cooldown is over
                    if (attackTimer <= 0)
                    {
                        PerformAttack();
                        attackTimer = attackCooldown;
                    }
                    break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void CheckPlayerVisibility()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerVisible = false;

        // Only check if within range
        if (distance <= detectionRange)
        {
            // Check if in field of view
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < viewAngle / 2)
            {
                // Cast ray to check for obstacles
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, distance, obstacleLayer))
                {
                    playerVisible = hit.collider.CompareTag("Player");
                }
                else
                {
                    playerVisible = true;
                }
            }

            // Always detect at very close range
            if (distance <= 1.5f)
            {
                playerVisible = true;
            }
        }
    }

    private void GeneratePatrolDestination()
    {
        // Generate random point within patrol radius
        //Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        //randomDirection.y = 0;
        //Vector3 targetPosition = startPosition + randomDirection;

        //NavMeshHit navHit;
        //if (NavMesh.SamplePosition(targetPosition, out navHit, patrolRadius, NavMesh.AllAreas))
        //{
        //    navAgent.SetDestination(navHit.position);
        //}
    }

    private IEnumerator SearchLastKnownPosition()
    {
        // Remember last known position
        Vector3 lastKnownPosition = playerTransform != null ? playerTransform.position : transform.position;
        navAgent.SetDestination(lastKnownPosition);

        // Wait a bit before returning to patrol
        yield return new WaitForSeconds(3f);

        // If player not found again, go back to patrol
        if (!playerVisible)
        {
            currentState = AIState.Patrol;
            GeneratePatrolDestination();
        }
    }

    private void LookAtPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
    }

    private void PerformAttack()
    {
        if (playerTransform == null) return;

        // Simple raycast attack
        Vector3 attackOrigin = transform.position + Vector3.up;
        Vector3 directionToPlayer = (playerTransform.position - attackOrigin).normalized;

        RaycastHit hit;
        if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Get player health component and deal damage
                Debug.Log("Hit player! Would apply damage here.");

                // Would normally call something like:
                // hit.collider.GetComponent<PlayerHealth>().TakeDamage(10);
            }
        }
    }

    private void DrawDebugVisuals()
    {
        // Draw FOV cone
        Debug.DrawLine(
            transform.position,
            transform.position + Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * detectionRange,
            Color.yellow
        );

        Debug.DrawLine(
            transform.position,
            transform.position + Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * detectionRange,
            Color.yellow
        );

        // Draw attack range
        Debug.DrawLine(transform.position, transform.position + transform.forward * attackRange, Color.red);
    }
}
