using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
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

        if (enemyModel.showFieldOfView)
        {
            CreateFOVMesh();
        }

        enemyModel.startPosition = transform.position;

        if (obstacleLayer == 0)
        {
            obstacleLayer = LayerMask.GetMask("Default", "Environment", "Wall");
        }
    }

    private void CreateFOVMesh()
    {
        //Membuat game object untuk visualisasi FOV
        GameObject fovObject = new GameObject("FOV_Visualizer");
        fovObject.transform.parent = transform;
        fovObject.transform.localPosition = Vector3.zero;
        fovObject.transform.localRotation = Quaternion.identity;

        //Mesh Filter dan Renderer
        MeshFilter meshFilter = fovObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = fovObject.AddComponent<MeshRenderer>();

        //Membuat material dengan shader yang transparant
        Material fovMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        fovMaterial.color = enemyModel.fieldOfViewColor;
        meshRenderer.material = fovMaterial;

        //iniliasi mesh
        fovMesh = new Mesh();
        meshFilter.mesh = fovMesh;
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

        if (enemyModel.showFieldOfView  && fovMesh != null)
        {
            UpdateFOVMesh();
        }
        // Update enemy movement based on NavMeshAgent if available
        UpdateMovement();

        // Debug state changes
        if (previousState != currentState)
        {
            Debug.Log($"Enemy state changed: {previousState} -> {currentState}");
            previousState = currentState;
        }
    }
    private void UpdateFOVMesh()
    {
        if (fovMesh == null) return;

        int segments = 50;
        float angle = enemyModel.viewAngle;
        float range = enemyModel.detectionRange;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int [segments * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i <=segments; i++)
        {
            float percent = i / (float)segments;
            float radian = percent * angle * Mathf.Deg2Rad;
            float x = Mathf.Sin(radian - (angle * Mathf.Deg2Rad / 2));
            float z = Mathf.Cos(radian - (angle * Mathf.Deg2Rad / 2));

            Vector3 direction = new Vector3(x, 0, z);
            direction = transform.TransformDirection(direction);

            //Check for obstacles

            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, direction, out hit, range, obstacleLayer))
            {
                vertices[i + 1] = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                vertices[i + 1] = transform.InverseTransformPoint(transform.position + Vector3.up + direction * range);
            }

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        fovMesh.Clear();
        fovMesh.vertices = vertices;
        fovMesh.triangles = triangles;
        fovMesh.RecalculateNormals();
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

    private bool IsInFieldOfView (Vector3 targetPosition)
    {
        if (!enemyModel.useFieldOfView)
            return true;

        // Mendapatkan arah menuju target
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        //Kalkulasi angle diantara arah kedepan dan arah menuju target
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        // Memastikan jika target di dalam pengelihatan angle
        if (angle < enemyModel.viewAngle / 2)
        {
            return true;
        }

        // Melihat jika target berada di pengelihatan periferal
        if (angle < (enemyModel.viewAngle / 2) + enemyModel.peripheralViewAngle)
        {
            //Berada di pengelihatan periferal, enemy mempunyai kesempatan untuk mendeteksi player menggunakan jarak
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            float detectionChance = 1.0f - (angle / ((enemyModel.viewAngle / 2) + enemyModel.peripheralViewAngle));

            //Peluang tinggi untuk mendeteksi player pada pengelihatan periferal
            detectionChance *= (enemyModel.detectionRange - distanceToTarget) / enemyModel.detectionRange;

            if (Random.value < detectionChance)
            {
                return true;
            }
        }

        // Pada jarak yang sangat dekat, selalu mendeteksi, berapa pun FOV-nya
        if (Vector3.Distance(transform.position, targetPosition) <= enemyModel.closeRangeAwareness)
        {
            
            return true;
        }
        return false;
    }

    private void CheckPlayerVisibility()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Reset visibility flags
        playerInSight = false;
        playerInFieldOfView = false;

        // Only check for visibility if player is within detection range
        if (distanceToPlayer <= enemyModel.detectionRange)
        {
            // First check if player is in field of view
            playerInFieldOfView = IsInFieldOfView(playerTransform.position);

            if (playerInFieldOfView)
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

                // Update line renderer positions for debugging
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

                // Very close range force detection for gameplay purposes
                if (distanceToPlayer <= enemyModel.closeRangeAwareness)
                {
                    playerInSight = true;
                }
            }
            else
            {
                // Player not in FOV, update line renderer to show nothing
                if (lineOfSight != null && enemyModel.showLineOfSight)
                {
                    lineOfSight.SetPosition(0, transform.position + Vector3.up);
                    lineOfSight.SetPosition(1, transform.position + Vector3.up);
                    lineOfSight.startColor = Color.gray;
                    lineOfSight.endColor = Color.gray;
                }
            }
        }
        else
        {
            // Player too far, update line renderer to show nothing
            if (lineOfSight != null && enemyModel.showLineOfSight)
            {
                lineOfSight.SetPosition(0, transform.position + Vector3.up);
                lineOfSight.SetPosition(1, transform.position + Vector3.up);
                lineOfSight.startColor = Color.gray;
                lineOfSight.endColor = Color.gray;
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
        if (playerInSight && playerInFieldOfView)
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
        if (playerInSight && playerInFieldOfView)
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
        if (!playerInSight || !playerInFieldOfView)
        {
            // For debugging, let's stay in chase mode for now
            StartCoroutine(LosingPlayerMemory());
             return;
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

    private IEnumerator LosingPlayerMemory()
    {
        // Enemy "remembers" dimana enemy terakhir melihat player untuk waktu yang singkat
        Vector3 lastKnownPosition = playerTransform.position;

        if (navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(lastKnownPosition);
        }

        // Menunggu untuk enemy mencapai lokasi terakhir atau timeout
        float searchTime = 0f;
        float maxSearchTime = 3f;

        while (searchTime < maxSearchTime)
        {
            // Jika player ditemukan lagi, kembali pada state pengejaran
            if (playerInSight && playerInFieldOfView)
            {
                yield break;
            }
            
            // Jika enemy mencapai waktu lokasi terakhir
            if (navAgent != null && !navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                break;
            }

            searchTime += 0.2f;
            yield return new WaitForSeconds(searchTime);
        }

        // Setelah pencarian sementara, kembali pada state patrol
        currentState = AIState.Patrol;
    }

    private void HandleAttackState()
    {
        if (playerTransform == null)
        {
            currentState = AIState.Patrol;
            return;
        }

        // Jika player itu menghilang dari pengelihatan atau diluar dari area pengelihatan, kembali untuk mengejar
        if (!playerInSight || !playerInFieldOfView)
        {
            currentState = AIState.Chase;
            if(navAgent !=null && navAgent.enabled)
            {
                navAgent.isStopped = false;
            }
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
        
        // Untuk enemy jarak pendek, menggunakan EnemyActive's attack method
        if (enemyModel.enemyType == EnemyType.EnemyShort && enemyActive != null)
        {
            if (enemyModel.attackTimer <= 0)
            {
                enemyActive.AttackPlayer();
                enemyModel.attackTimer = enemyModel.attackCooldown;
            }
        }
        // Untuk enemy jarak jauh, menggunakan cara sendiri
        else if (enemyModel.enemyType == EnemyType.EnemyRange)
        {
            // Melakukan attack jika cooldown selesai
            if (enemyModel.attackTimer <= 0)
            {
                PerformAttackRange();
                enemyModel.attackTimer = enemyModel.attackCooldown;
            }

            // Chance to retreat after attack (especially for ranged enemies)
            if (Random.value < 0.3f)
            {
                currentState = AIState.Retreat;
                if (navAgent != null && navAgent.enabled)
                {
                    navAgent.isStopped = false;
                }
            }
        }
        //// Perform attack if cooldown is over
        //if (enemyModel.attackTimer <= 0)
        //{
        //    PerformAttackRange();
        //    enemyModel.attackTimer = enemyModel.attackCooldown;
        //}

        //// Chance to retreat after attack (especially for ranged enemies)
        //if (enemyModel.enemyType == EnemyType.EnemyRange && Random.value < 0.3f)
        //{
        //    currentState = AIState.Retreat;
        //    if (navAgent != null && navAgent.enabled)
        //    {
        //        navAgent.isStopped = false;
        //    }
        //}
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
            currentState = playerInSight && playerInFieldOfView ? AIState.Chase : AIState.Patrol;
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

    public void PerformAttackRange()
    {
        // For range enemies, use raycast to attack
        enemyModel.isAttacking = true;
        // For range enemies, use raycast to attack
        if (enemyModel.enemyType == EnemyType.EnemyRange)
        {
            Vector3 targetPoint;
            Vector3 attackOrigin = transform.position + Vector3.up * 1.5f; // Adjust to fire from head/weapon height
            Vector3 directionToPlayer = (playerTransform.position + Vector3.up * 1.0f - attackOrigin).normalized; // Aim at player's center mass
            RaycastHit hit;

            // Visual effect for ranged attack
            Debug.DrawRay(attackOrigin, 3 * enemyModel.attackRange * directionToPlayer, Color.magenta, 1.0f);

            // Create a temporary LineRenderer for the attack visualization with better properties
           
            // Perform the actual raycast
            if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, enemyModel.attackRange * 3, hitLayer))
            {
                targetPoint = hit.point;
                // Check if we hit the player
                if (hit.collider.CompareTag("Player"))
                {
                    // Apply damage to player - using TryGetComponent for efficiency
                    hit.collider.gameObject.TryGetComponent<PlayerActive>(out var playerActive);
                    if (playerActive != null)
                    {
                        playerActive.TakeDamage(enemyModel.attackPower);
                        Debug.Log($"Range attack hit player for {enemyModel.attackPower} damage");
                    }
                    else
                    {
                        Debug.LogWarning("Player hit but no PlayerHealth component found");
                    }

                    // Create hit effect for player
                    if (Resources.Load<GameObject>("Prefabs/HitEffect"))
                    {
                        Instantiate(Resources.Load<GameObject>("Prefabs/HitEffect"), hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
                else
                {
                    //targetPoint = Vector3.forward;
                }
            }
            else
            {
                targetPoint = attackOrigin + (directionToPlayer * enemyModel.attackRange * 3);
            }
            StartCoroutine(BulletTrailEffect(targetPoint, attackOrigin));
            StartCoroutine(ResetAttackFlag());
        }
    }
    public IEnumerator BulletTrailEffect(Vector3 targetPoint, Vector3 startPoint)
    {
        // Buat LineRenderer khusus untuk efek tembakan
        LineRenderer bulletTrail = gameObject.AddComponent<LineRenderer>();
        bulletTrail.startWidth = 0.05f;
        bulletTrail.endWidth = 0.05f;
        bulletTrail.material = new Material(Shader.Find("Sprites/Default"));
        bulletTrail.startColor = Color.red;
        bulletTrail.endColor = Color.yellow;
        bulletTrail.positionCount = 2;

        // Set posisi bullet trail
        bulletTrail.SetPosition(0, startPoint);
        bulletTrail.SetPosition(1, targetPoint);

        // Tunggu beberapa saat
        yield return new WaitForSeconds(0.1f);

        // Hapus efek trail
        Destroy(bulletTrail);
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

        // Menunjukkan Cone FOV
        if (enemyModel.useFieldOfView)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange

            float halfFOV = enemyModel.viewAngle / 2;
            float coneLength = enemyModel.detectionRange;

            // Draw main FOV cone
            Vector3 forward = transform.forward * coneLength;
            Vector3 right = Quaternion.Euler(0, halfFOV, 0) * forward;
            Vector3 left = Quaternion.Euler(0, halfFOV, 0) * forward;

            Gizmos.DrawLine(transform.position, transform.position + right);
            Gizmos.DrawLine(transform.position, transform.position + left);
            Gizmos.DrawLine(transform.position + right, transform.position + left);

            // Draw peripheral vision extension if enabled
            if (enemyModel.peripheralViewAngle > 0)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f); // Lighter orange

                float halfPeripheralFOV = (enemyModel.viewAngle / 2) + enemyModel.peripheralViewAngle;
                float peripheralConeLength = enemyModel.detectionRange * 0.7f; // Shorter range for peripheral vision

                Vector3 peripheralForward = transform.forward * peripheralConeLength;
                Vector3 peripheralRight = Quaternion.Euler(0, halfPeripheralFOV, 0) * peripheralForward;
                Vector3 peripheralLeft = Quaternion.Euler(0, -halfPeripheralFOV, 0) * peripheralForward;

                Gizmos.DrawLine(transform.position, transform.position + peripheralRight);
                Gizmos.DrawLine(transform.position, transform.position + peripheralLeft);
                Gizmos.DrawLine(transform.position + peripheralRight, transform.position + peripheralLeft);
            }

            // Draw close range awareness circle
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // Red with transparency
            Gizmos.DrawSphere(transform.position, enemyModel.closeRangeAwareness);
        }

        // Draw current destination if in play mode
        if (Application.isPlaying && navAgent != null && navAgent.enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(navAgent.destination, 0.3f);
            Gizmos.DrawLine(transform.position, navAgent.destination);
        }

    }

}

