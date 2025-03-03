using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyModel : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("References")]
    private EnemyActive enemyActive;
    private NavMeshAgent navAgent;
    private LineRenderer lineOfSight;

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

    [Header("Debug Visualization")]
    public bool showLineOfSight = true;
    public Color lineOfSightColor = Color.red;

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
    private AIState previousState;

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

        // Create line renderer for line of sight visualization
        if (showLineOfSight && lineOfSight == null)
        {
            lineOfSight = gameObject.AddComponent<LineRenderer>();
            lineOfSight.startWidth = 0.1f;
            lineOfSight.endWidth = 0.1f;
            lineOfSight.material = new Material(Shader.Find("Sprites/Default"));
            lineOfSight.startColor = lineOfSightColor;
            lineOfSight.endColor = lineOfSightColor;
            lineOfSight.positionCount = 2;
        }

        startPosition = transform.position;
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
            Debug.LogError("Failed to initialize player transform in EnemyModel!");
        }

        // Configure NavMeshAgent based on enemy type
        if (navAgent != null)
        {
            switch (enemyType)
            {
                case EnemyType.EnemyShort:
                    navAgent.stoppingDistance = attackRange * 0.8f;
                    break;

                case EnemyType.EnemyRange:
                    navAgent.stoppingDistance = attackRange * 2.5f;  // Range enemies stay further away
                    break;
            }
        }

        // Start the enemy AI behavior
        StartCoroutine(UpdateAI());
    }

    void Update()
    {
        if (isDeath) return;

        // Make sure we have the player
        if (playerTransform == null && enemyActive != null && enemyActive.Player != null)
        {
            playerTransform = enemyActive.Player;
        }

        // Update timers
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (destinationChangeTimer > 0)
            destinationChangeTimer -= Time.deltaTime;

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
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
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
            if (lineOfSight != null && showLineOfSight)
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
            if (distanceToPlayer <= detectionRange * 0.7f)
            {
                playerInSight = true;
            }
        }
        else
        {
            playerInSight = false;

            // Update line renderer to show nothing
            if (lineOfSight != null && showLineOfSight)
            {
                lineOfSight.SetPosition(0, transform.position + Vector3.up);
                lineOfSight.SetPosition(1, transform.position + Vector3.up);
            }
        }
    }

    private IEnumerator UpdateAI()
    {
        while (!isDeath)
        {
            // Force visible detection if player is close
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                if (distanceToPlayer <= detectionRange * 0.5f)
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
        float effectiveAttackRange = (enemyType == EnemyType.EnemyRange) ? attackRange * 2.5f : attackRange;

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
        float effectiveAttackRange = (enemyType == EnemyType.EnemyRange) ? attackRange * 2.5f : attackRange;

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
        if (attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }

        // Chance to retreat after attack (especially for ranged enemies)
        if (enemyType == EnemyType.EnemyRange && Random.value < 0.3f)
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
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0; // Keep on the same Y level
        Vector3 targetPosition = startPosition + randomDirection;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(targetPosition, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            currentDestination = navHit.position;
            navAgent.SetDestination(currentDestination);
        }
    }

    private void PerformAttack()
    {
        // For range enemies, use raycast to attack
        if (enemyType == EnemyType.EnemyRange)
        {
            Vector3 attackOrigin = transform.position + Vector3.up * 1.5f; // Adjust to fire from head/weapon height
            Vector3 directionToPlayer = (playerTransform.position + Vector3.up * 1.0f - attackOrigin).normalized; // Aim at player's center mass
            RaycastHit hit;

            // Visual effect for ranged attack
            Debug.DrawRay(attackOrigin, directionToPlayer * attackRange * 3, Color.magenta, 1.0f);

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
            if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, attackRange * 3, layerMask))
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

                    // Apply damage to player - using TryGetComponent for efficiency
                    //var playerHealth = hit.collider.gameObject.GetComponent<PlayerHealth>();
                    //if (playerHealth != null)
                    //{
                    //    playerHealth.TakeDamage(attackPower);
                    //    Debug.Log($"Range attack hit player for {attackPower} damage");
                    //}
                    //else
                    //{
                    //    Debug.LogWarning("Player hit but no PlayerHealth component found");
                    //}

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
                Vector3 endPoint = attackOrigin + directionToPlayer * attackRange * 3;
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
            if (navAgent != null)
            {
                StartCoroutine(ApplyRecoil(-directionToPlayer * 0.5f));
            }
        }

        // Helper method for recoil effect
        IEnumerator ApplyRecoil(Vector3 recoilDirection)
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
        IEnumerator ResetAttackFlag()
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
}