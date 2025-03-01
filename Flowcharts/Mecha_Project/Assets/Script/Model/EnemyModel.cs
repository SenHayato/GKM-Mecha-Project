using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyModel : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("References")]
    private EnemyActive enemyActive;
    private NavMeshAgent navAgent;

    [Header("Health")]
    public int maxHealth;
    public int health;
    public int minHealth;

    [Header("Attack")]
    public int attackPower;
    public int attackSpeed;
    public float attackRange = 2f;
    public float detectionRange = 10f;

    [Header("Status")]
    public bool isAttacking;
    public bool isDeath;
    public bool isMoving;
    public bool isBlocking;

    [Header("AI Behavior")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    private Vector3 startPosition;
    private Vector3 currentDestination;
    private float destinationChangeTimer = 0f;

    [Header("Combat")]
    public float attackCooldown = 2f;
    private float attackTimer = 0f;

    // State management
    private enum AIState { Idle, Patrol, Chase, Attack, Retreat, Dead }
    private AIState currentState = AIState.Idle;

    private Transform playerTransform;
    private bool playerInSight = false;
    private bool isObstacleInTheWay = false;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();

        // If there's no NavMeshAgent, add one
        if (navAgent == null && Application.isPlaying)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
            navAgent.speed = 3.5f;
            navAgent.acceleration = 8.0f;
            navAgent.angularSpeed = 120f;
        }

        startPosition = transform.position;
    }

    public void Initialize(EnemyActive active)
    {
        enemyActive = active;

        if (enemyActive != null)
        {
            playerTransform = enemyActive.Player;
        }

        // Configure NavMeshAgent based on enemy type
        if (navAgent != null)
        {
            switch (enemyType)
            {
                case EnemyType.EnemyShort:
                    navAgent.stoppingDistance = attackRange;
                    break;

                case EnemyType.EnemyRange:
                    navAgent.stoppingDistance = attackRange * 3;  // Range enemies stay further away
                    break;
            }
        }

        // Start the enemy AI behavior
        StartCoroutine(UpdateAI());
    }

    void Update()
    {
        if (isDeath) return;

        // Update timers
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (destinationChangeTimer > 0)
            destinationChangeTimer -= Time.deltaTime;

        // Update enemy movement based on NavMeshAgent if available
        UpdateMovement();

        // Check if player is in sight (simple line of sight check)
        CheckPlayerVisibility();
    }

    private void UpdateMovement()
    {
        if (navAgent != null && navAgent.enabled)
        {
            // If we have a NavMeshAgent, let it handle movement
            isMoving = navAgent.velocity.magnitude > 0.1f;

            // Sync character controller with NavMeshAgent
            if (enemyActive != null)
            {
                Vector3 direction = navAgent.desiredVelocity.normalized;
                float currentSpeed = navAgent.speed;

                // Only apply movement through EnemyActive if we need to move
                if (isMoving)
                {
                    enemyActive.ApplyMovement(direction, currentSpeed, true);
                }
                else
                {
                    enemyActive.ApplyMovement(Vector3.zero, 0, false);
                }
            }
        }
    }

    private void CheckPlayerVisibility()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Only check for visibility if player is within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Simple line of sight check
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            isObstacleInTheWay = Physics.Raycast(
                transform.position + Vector3.up,
                directionToPlayer,
                distanceToPlayer,
                Physics.AllLayers ^ (1 << LayerMask.NameToLayer("Enemy"))
            );

            playerInSight = !isObstacleInTheWay;
        }
        else
        {
            playerInSight = false;
        }
    }

    private IEnumerator UpdateAI()
    {
        while (!isDeath)
        {
            switch (currentState)
            {
                case AIState.Idle:
                    HandleIdleState();
                    break;

                case AIState.Patrol:
                    HandlePatrolState();
                    break;

                case AIState.Chase:
                    HandleChaseState();
                    break;

                case AIState.Attack:
                    HandleAttackState();
                    break;

                case AIState.Retreat:
                    HandleRetreatState();
                    break;

                case AIState.Dead:
                    // Do nothing in dead state
                    break;
            }

            // Wait before next AI update to avoid excessive computation
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void HandleIdleState()
    {
        // If player is spotted, chase them
        if (playerInSight)
        {
            currentState = AIState.Chase;
            return;
        }

        // Randomly decide to patrol
        if (Random.value < 0.3f)
        {
            currentState = AIState.Patrol;
            GeneratePatrolDestination();
        }
    }

    private void HandlePatrolState()
    {
        // If player is spotted, chase them
        if (playerInSight)
        {
            currentState = AIState.Chase;
            return;
        }

        // Check if we need a new patrol destination
        if (destinationChangeTimer <= 0f ||
            (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f))
        {
            destinationChangeTimer = patrolWaitTime;

            // Either go idle or find a new patrol point
            if (Random.value < 0.3f)
            {
                currentState = AIState.Idle;
                if (navAgent != null)
                    navAgent.SetDestination(transform.position);
            }
            else
            {
                GeneratePatrolDestination();
            }
        }
    }

    private void HandleChaseState()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        // If player is lost from sight for a while, go back to patrol
        if (!playerInSight)
        {
            currentState = AIState.Patrol;
            return;
        }

        // Move toward player
        if (navAgent != null)
        {
            navAgent.SetDestination(playerTransform.position);
        }

        // Check if in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float effectiveAttackRange = (enemyType == EnemyType.EnemyRange) ? attackRange * 3 : attackRange;

        if (distanceToPlayer <= effectiveAttackRange)
        {
            currentState = AIState.Attack;
        }
    }

    private void HandleAttackState()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        // Face the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);

        // Check if we're still in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float effectiveAttackRange = (enemyType == EnemyType.EnemyRange) ? attackRange * 3 : attackRange;

        if (distanceToPlayer > effectiveAttackRange)
        {
            currentState = AIState.Chase;
            return;
        }

        // Perform attack if cooldown is over
        if (attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }

        // Chance to retreat after attack (especially for ranged enemies)
        if (enemyType == EnemyType.EnemyRange && Random.value < 0.3f)
        {
            currentState = AIState.Retreat;
        }
    }

    private void HandleRetreatState()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        // Move away from player
        Vector3 directionFromPlayer = (transform.position - playerTransform.position).normalized;
        Vector3 retreatPosition = transform.position + directionFromPlayer * 5f;

        // Find a valid retreat position on the NavMesh
        if (navAgent != null)
        {
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(retreatPosition, out navHit, 10f, NavMesh.AllAreas))
            {
                navAgent.SetDestination(navHit.position);
            }
        }

        // After a short retreat, go back to patrolling or chasing
        if (Random.value < 0.3f ||
            (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f))
        {
            currentState = playerInSight ? AIState.Chase : AIState.Patrol;
        }
    }

    private void GeneratePatrolDestination()
    {
        if (navAgent == null) return;

        // Generate a random point within patrol radius
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += startPosition;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            currentDestination = navHit.position;
            navAgent.SetDestination(currentDestination);
        }
    }

    private void PerformAttack()
    {
        if (enemyActive == null) return;

        isAttacking = true;

        // Trigger attack animation
        Animator anim = enemyActive.anim;
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // For range enemies, instantiate a projectile
        if (enemyType == EnemyType.EnemyRange)
        {
            // You could implement projectile instantiation here
            // For now, we'll just do a raycast hit
            if (playerTransform != null)
            {
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                RaycastHit hit;

                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, attackRange * 3))
                {
                    // Check if we hit the player
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Apply damage to player - you would add your player damage method here
                        // hit.collider.GetComponent<PlayerHealth>().TakeDamage(attackPower);
                        Debug.Log("Range attack hit player for " + attackPower + " damage");
                    }
                }
            }
        }
        else
        {
            // For melee enemies, check for player collision
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * attackRange / 2, attackRange / 2);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    // Apply damage to player - you would add your player damage method here
                    // hitCollider.GetComponent<PlayerHealth>().TakeDamage(attackPower);
                    Debug.Log("Melee attack hit player for " + attackPower + " damage");
                    break;
                }
            }
        }

        // Reset attack flag - the animation event will also do this
        StartCoroutine(ResetAttackFlag());
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // Optional: Visual debugging
    void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw patrol area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, patrolRadius);
    }
}

public enum EnemyType
{
    EnemyShort,  // Melee enemies
    EnemyRange   // Ranged enemies
}