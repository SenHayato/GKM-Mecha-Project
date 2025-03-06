using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private EnemyModel enemyModel;
    private EnemyActive enemyActive;
    private NavMeshAgent navAgent;
    public LineRenderer lineOfSight;

    // State management
    private enum AIState { Idle, Patrol, Chase, Attack, Retreat, Dead }
    private AIState currentState = AIState.Idle;
    private AIState previousState;

    private Transform playerTransform;
    private bool playerInSight = false;
    private bool isObstacleInTheWay = false;



    void Awake()
    {
        enemyModel = GetComponent<EnemyModel>();
        navAgent = GetComponent<NavMeshAgent>();

        // If there's no NavMeshAgent, add one
        if (navAgent == null && Application.isPlaying)
        {
            navAgent = gameObject.AddComponent<NavMeshAgent>();
            navAgent.speed = 3.5f;
            navAgent.acceleration = 8.0f;
            navAgent.angularSpeed = 120f;
        }

        // Create line renderer for line of sight visualization
        if (enemyModel.showLineOfSight && lineOfSight == null)
        {
            lineOfSight = gameObject.AddComponent<LineRenderer>();
            lineOfSight.startWidth = 0.1f;
            lineOfSight.endWidth = 0.1f;
            lineOfSight.material = new Material(Shader.Find("Sprites/Default"));
            lineOfSight.startColor = enemyModel.lineOfSightColor;
            lineOfSight.endColor = enemyModel.lineOfSightColor;
            lineOfSight.positionCount = 2;
        }

        enemyModel.startPosition = transform.position;
    }

    public void Initialize(EnemyActive active)
    {
        enemyActive = active;

        if (enemyActive != null && enemyActive.Player != null)
        {
            playerTransform = enemyActive.Player;
            Debug.Log("Player transform initialized: " + playerTransform.name);
        }
        else
        {
            Debug.LogError("Failed to initialize player transform in EnemyAIController!");
        }

        // Configure NavMeshAgent based on enemy type
        if (navAgent != null)
        {
            switch (enemyModel.enemyType)
            {
                case EnemyType.EnemyShort:
                    navAgent.stoppingDistance = enemyModel.attackRange * 0.8f;
                    break;

                case EnemyType.EnemyRange:
                    navAgent.stoppingDistance = enemyModel.attackRange * 2.5f;  // Range enemies stay further away
                    break;
            }
        }

        // Start the enemy AI behavior
        StartCoroutine(UpdateAI());
    }

    void Update()
    {
        if (enemyModel.isDeath) return;

        // Make sure we have the player
        if (playerTransform == null && enemyActive != null && enemyActive.Player != null)
        {
            playerTransform = enemyActive.Player;
        }

        // Update timers
        if (enemyModel.attackTimer > 0)
            enemyModel.attackTimer -= Time.deltaTime;

        if (enemyModel.destinationChangeTimer > 0)
            enemyModel.destinationChangeTimer -= Time.deltaTime;

        // Check if player is in sight
        CheckPlayerVisibility();

        // Update enemy movement based on NavMeshAgent if available
        UpdateMovement();

        // Debug state changes
        if (previousState != currentState)
        {
            Debug.Log($"Enemy state changed: {previousState} -> {currentState}");
            previousState = currentState;
        }

        // Force state to chase if in range for debugging
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < enemyModel.detectionRange)
        {
            if (currentState != AIState.Chase && currentState != AIState.Attack)
            {
                Debug.Log("Player in range, switching to Chase state");
            }
        }
    }

    private void UpdateMovement()
    {
        if (navAgent != null && navAgent.enabled)
        {
            // If we have a NavMeshAgent, let it handle movement
            enemyModel.isMoving = navAgent.velocity.magnitude > 0.1f;

            // Sync character controller with NavMeshAgent
            if (enemyActive != null)
            {
                Vector3 direction = navAgent.desiredVelocity.normalized;
                float currentSpeed = navAgent.speed;

                // Only apply movement through EnemyActive if we need to move
                if (enemyModel.isMoving)
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
        if (distanceToPlayer <= enemyModel.detectionRange)
        {
            // Set start position for line of sight slightly above the enemy's center
            Vector3 startPos = transform.position + Vector3.up;

            // Calculate direction to player and target position
            Vector3 directionToPlayer = (playerTransform.position - startPos).normalized;
            Vector3 targetPos = playerTransform.position;

            // Raycast to check for obstacles
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(
                startPos,
                directionToPlayer,
                out hit,
                distanceToPlayer,
                Physics.AllLayers ^ (1 << LayerMask.NameToLayer("Enemy"))
            );

            // Update line renderer positions
            if (lineOfSight != null && enemyModel.showLineOfSight)
            {
                lineOfSight.SetPosition(0, startPos);

                if (hitSomething)
                {
                    // If we hit something, draw line to hit point
                    lineOfSight.SetPosition(1, hit.point);

                    // If we hit the player, change line color to green
                    if (hit.collider.CompareTag("Player"))
                    {
                        lineOfSight.startColor = Color.green;
                        lineOfSight.endColor = Color.green;
                        playerInSight = true;
                    }
                    else
                    {
                        // Hit something else
                        lineOfSight.startColor = Color.red;
                        lineOfSight.endColor = Color.red;
                        playerInSight = false;
                    }
                }
                else
                {
                    // No hit, draw full line
                    lineOfSight.SetPosition(1, targetPos);
                    lineOfSight.startColor = Color.yellow;
                    lineOfSight.endColor = Color.yellow;
                    playerInSight = true;
                }
            }
            else
            {
                // No line renderer, just update playerInSight based on hit
                playerInSight = hitSomething && hit.collider.CompareTag("Player");
            }

            // For debugging
            if (playerInSight)
            {
                Debug.DrawLine(startPos, targetPos, Color.green);
            }
            else
            {
                Debug.DrawLine(startPos, hitSomething ? hit.point : targetPos, Color.red);
            }

            // If player is in range, force detection for debugging
            if (distanceToPlayer <= enemyModel.detectionRange * 0.7f)
            {
                playerInSight = true;
            }
        }
        else
        {
            playerInSight = false;

            // Update line renderer to show nothing
            if (lineOfSight != null && enemyModel.showLineOfSight)
            {
                lineOfSight.SetPosition(0, transform.position + Vector3.up);
                lineOfSight.SetPosition(1, transform.position + Vector3.up);
            }
        }
    }

    private IEnumerator UpdateAI()
    {
        while (!enemyModel.isDeath)
        {
            // Force visible detection if player is close
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer <= enemyModel.detectionRange * 0.5f)
                {
                    playerInSight = true;
                }
            }

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
        if (enemyModel.destinationChangeTimer <= 0f ||
            (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f))
        {
            enemyModel.destinationChangeTimer = enemyModel.patrolWaitTime;

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
            // For debugging, let's stay in chase mode for now
            // currentState = AIState.Patrol;
            // return;
        }

        // Move toward player
        if (navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(playerTransform.position);

            // Debug log to confirm chase is working
            Debug.DrawLine(transform.position, playerTransform.position, Color.blue, 0.2f);
        }

        // Check if in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float effectiveAttackRange = (enemyModel.enemyType == EnemyType.EnemyRange) ? enemyModel.attackRange * 2.5f : enemyModel.attackRange;

        if (distanceToPlayer <= effectiveAttackRange)
        {
            currentState = AIState.Attack;
            Debug.Log("Switching to Attack state: distance = " + distanceToPlayer + ", attack range = " + effectiveAttackRange);
        }
    }

    private void HandleAttackState()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        // Stop movement
        if (navAgent != null && navAgent.enabled)
        {
            navAgent.isStopped = true;
        }

        // Face the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        }

        // Check if we're still in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float effectiveAttackRange = (enemyModel.enemyType == EnemyType.EnemyRange) ? enemyModel.attackRange * 2.5f : enemyModel.attackRange;

        if (distanceToPlayer > effectiveAttackRange)
        {
            currentState = AIState.Chase;
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.isStopped = false;
            }
            return;
        }

        // Perform attack if cooldown is over
        if (enemyModel.attackTimer <= 0)
        {
            PerformAttack();
            enemyModel.attackTimer = enemyModel.attackCooldown;
        }

        // Chance to retreat after attack (especially for ranged enemies)
        if (enemyModel.enemyType == EnemyType.EnemyRange && Random.value < 0.3f)
        {
            currentState = AIState.Retreat;
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.isStopped = false;
            }
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
        if (navAgent != null && navAgent.enabled)
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
        if (navAgent == null || !navAgent.enabled) return;

        // Generate a random point within patrol radius
        Vector3 randomDirection = Random.insideUnitSphere * enemyModel.patrolRadius;
        randomDirection.y = 0; // Keep on the same Y level
        Vector3 targetPosition = enemyModel.startPosition + randomDirection;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(targetPosition, out navHit, enemyModel.patrolRadius, NavMesh.AllAreas))
        {
            enemyModel.currentDestination = navHit.position;
            navAgent.SetDestination(enemyModel.currentDestination);
        }
    }

    private void PerformAttack()
    {
        // For range enemies, use raycast to attack
        enemyModel.isAttacking = true;

        // For range enemies, use raycast to attack
        if (enemyModel.enemyType == EnemyType.EnemyRange)
        {
            if (playerTransform != null)
            {
                Vector3 facingDirection = (playerTransform.position - transform.position).normalized;
                facingDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(facingDirection);
            }
            // Use the weapon fire point instead of a hardcoded offset
            Vector3 attackOrigin = enemyModel.weaponFirePoint != null
                ? enemyModel.weaponFirePoint.position
                : transform.position + transform.forward * 0.5f + Vector3.up * 1.2f + transform.right * 0.3f;

            Vector3 directionToPlayer = (playerTransform.position + Vector3.up * 1.0f - attackOrigin).normalized;
            RaycastHit hit;
            // Create a temporary LineRenderer for the attack visualization with better properties
            GameObject tempLaser = new GameObject("AttackLaser");
            LineRenderer laser = tempLaser.AddComponent<LineRenderer>();
            laser.startWidth = 0.1f;
            laser.endWidth = 0.05f; // Tapered effect

            // Use a more suitable material for beam effect
            if (Resources.Load<Material>("Materials/LaserBeam"))
            {
                laser.material = Resources.Load<Material>("Materials/LaserBeam");
            }
            else
            {
                Material laserMat = new Material(Shader.Find("Sprites/Default"));
                laserMat.color = Color.magenta;
                laser.material = laserMat;
            }

            laser.startColor = new Color(1f, 0f, 1f, 0.8f); // Semi-transparent magenta
            laser.endColor = new Color(1f, 0f, 1f, 0.2f); // Fade out at the end
            laser.positionCount = 2;
            laser.SetPosition(0, attackOrigin);

            // Add subtle glow effect
            GameObject glowObj = new GameObject("AttackGlow");
            glowObj.transform.position = attackOrigin;
            Light glowLight = glowObj.AddComponent<Light>();
            glowLight.color = Color.magenta;
            glowLight.intensity = 2f;
            glowLight.range = 3f;

            // Sound effect
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null && Resources.Load<AudioClip>("Sounds/LaserShot"))
            {
                audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/LaserShot"));
            }

            // Layer mask to ignore self/other enemies
            int layerMask = ~(1 << gameObject.layer);

            bool hitPlayer = false;

            // Perform the actual raycast
            if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, enemyModel.attackRange * 3, layerMask))
            {
                // Set endpoint of visual laser
                laser.SetPosition(1, hit.point);

                // Impact effect at hit point
                GameObject impactObj = new GameObject("ImpactEffect");
                impactObj.transform.position = hit.point;
                Light impactLight = impactObj.AddComponent<Light>();
                impactLight.color = Color.red;
                impactLight.intensity = 3f;
                impactLight.range = 2f;
                Destroy(impactObj, 0.3f);

                // Check if we hit the player
                if (hit.collider.CompareTag("Player"))
                {
                    hitPlayer = true;
                    if (hit.collider.TryGetComponent<PlayerActive>(out var player))
                    {
                        player.TakeDamage(enemyModel.attackPower);
                        Debug.Log("Player Terkena Damage : ");
                    }

                    // Create hit effect for player
                    if (Resources.Load<GameObject>("Prefabs/HitEffect"))
                    {
                        Instantiate(Resources.Load<GameObject>("Prefabs/HitEffect"), hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
                else
                {
                    // Hit something else - create appropriate effect based on material
                    // Create a simple particle effect at the hit point
                    GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.localScale = Vector3.one * 0.2f;

                    // Make it glow based on the surface hit
                    Renderer renderer = hitEffect.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material mat = new Material(Shader.Find("Standard"));
                        mat.EnableKeyword("_EMISSION");

                        // Adapt color based on what was hit
                        if (hit.collider.gameObject.CompareTag("Metal"))
                            mat.SetColor("_EmissionColor", Color.yellow * 2f);
                        else if (hit.collider.gameObject.CompareTag("Stone"))
                            mat.SetColor("_EmissionColor", Color.gray * 2f);
                        else
                            mat.SetColor("_EmissionColor", Color.red * 2f);

                        renderer.material = mat;
                    }

                    // Remove collider from hit effect
                    Collider hitCollider = hitEffect.GetComponent<Collider>();
                    if (hitCollider != null) Destroy(hitCollider);

                    // Destroy the hit effect after a delay
                    Destroy(hitEffect, 0.5f);
                }
            }
            else
            {
                // Didn't hit anything, show max range with effect at the end
                Vector3 endPoint = attackOrigin + directionToPlayer * enemyModel.attackRange * 3;
                laser.SetPosition(1, endPoint);

                // Optional: Add small end effect for missed shots
                GameObject endEffect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                endEffect.transform.position = endPoint;
                endEffect.transform.localScale = Vector3.one * 0.1f;

                Renderer renderer = endEffect.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = new Color(1f, 0f, 1f, 0.3f);
                    renderer.material = mat;
                }

                Collider endCollider = endEffect.GetComponent<Collider>();
                if (endCollider != null) Destroy(endCollider);

                Destroy(endEffect, 0.3f);
            }

            // Add muzzle flash effect at the origin
            GameObject muzzleFlash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            muzzleFlash.transform.position = attackOrigin;
            muzzleFlash.transform.localScale = Vector3.one * 0.3f;

            Renderer muzzleRenderer = muzzleFlash.GetComponent<Renderer>();
            if (muzzleRenderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * 3f);
                muzzleRenderer.material = mat;
            }

            Collider muzzleCollider = muzzleFlash.GetComponent<Collider>();
            if (muzzleCollider != null) Destroy(muzzleCollider);

            // Destroy temporary visual effects after delay
            Destroy(muzzleFlash, 0.2f);
            Destroy(tempLaser, hitPlayer ? 0.3f : 0.5f); // Shorter duration if hit for more responsive feel
            Destroy(glowObj, 0.2f);

            // Apply a slight recoil to the enemy
        }

        StartCoroutine(ResetAttackFlag());
    }

    private void LookAtPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                if (enemyModel.enemyType == EnemyType.EnemyRange && currentState == AIState.Attack && enemyModel.attackTimer <= 0f)
                {
                    transform.rotation = Quaternion.LookRotation(direction);

                    Debug.Log("Ranged enemy directly facing player for attack");
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
                }
            }
        }
    }
        

    // Helper method for recoil effect
    private IEnumerator ApplyRecoil(Vector3 recoilDirection)
    {
        if (navAgent != null && navAgent.enabled)
        {
            // Temporarily disable NavMeshAgent to allow manual movement
            navAgent.isStopped = true;
            navAgent.updatePosition = false;

            // Apply small recoil push
            Vector3 originalPos = transform.position;
            Vector3 targetPos = transform.position + recoilDirection;

            float elapsed = 0f;
            float duration = 0.1f;

            while (elapsed < duration)
            {
                transform.position = Vector3.Lerp(originalPos, targetPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset NavMeshAgent
            navAgent.nextPosition = transform.position;
            navAgent.updatePosition = true;
            navAgent.isStopped = false;
        }

        yield return null;
    }

    private IEnumerator ResetAttackFlag()
    {
        yield return new WaitForSeconds(0.5f);
        enemyModel.isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (enemyModel == null) return;

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyModel.attackRange);

        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyModel.detectionRange);

        // Draw patrol area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Application.isPlaying ? enemyModel.startPosition : transform.position, enemyModel.patrolRadius);
    }
}
