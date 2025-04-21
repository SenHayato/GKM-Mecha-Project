using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class ControllerEnemy : MonoBehaviour
{
    // State Management
    private enum AIState { Idle, Patrol, Chase, Attack, Dead}
    private AIState currentState = AIState.Idle;
    private AIState previousState;

    // Components
    public LayerMask hitLayer;
    private NavMeshAgent agent;
    private Transform playerTransform;
    private EnemyModel model;
    private EnemyActive active;
    private Animator anim;
    CharacterController enemy;

    [SerializeField]
    private Transform[] wayPoints;
    private int currentWaypoints;

    private bool playerInSight = false;
    private bool playerInFieldOfView = false;
    private bool isObstacleInTheWay = false;
    private bool isAlerted = false;

    private void Awake()
    {
        active = GetComponent<EnemyActive>();
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        model = GetComponent<EnemyModel>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentWaypoints = -1;
        anim = GetComponent<Animator>();

        GoToNexPoint();

    }

    private void Update()
    {
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        // Patrolling
        //switch (currentState)
        //{
        //    case AIState.Patrol:
        //        HandlePatrol();
        //            if(distance < model.detectionRange)
        //        {
        //            currentState = AIState.Chase;
        //        }
        //        break;
        //    case AIState.Chase:
        //        HandleChase(distance);
        //        if(distance < agent.stoppingDistance)
        //        {
        //            currentState = AIState.Attack;
        //        }else if (distance > model.detectionRange + 2f)
        //        {
        //            currentState = AIState.Patrol;
        //        }
        //        break;
        //        case AIState.Attack:
        //        HandleAttack(distance);
        //        if (distance > agent.stoppingDistance)
        //        {
        //            currentState = AIState.Chase;
        //        }
        //        else
        //        {
        //            HandleAttack(distance);
        //        }
        //        break;
                
        //}
        if (distance < model.alertRange)
        {
            if (!isAlerted)
            {
                isAlerted = true;
                anim.SetTrigger("Equip");
            }
            
            agent.isStopped = true;
            agent.SetDestination(playerTransform.position);
            anim.SetBool("isRunning", false);
        }
        else
        {
            if (isAlerted)
            {
                isAlerted = false;
                GoToNexPoint();
            }

            if(!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNexPoint();
            }
        
        }
        // Update timers
        if (model.attackTimer > 0)
            model.attackTimer -= Time.deltaTime;

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

   
    #region StateAI
    void HandlePatrol()
    {
        if (agent.remainingDistance < 0.5f)
        {
            if (model.maxWaitingTime == 0)
                model.maxWaitingTime = 5f;
            if (agent.radius <= model.alertRange)
            {
                active.Equip();
            }
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
    }

    void HandleChase(float distance)
    {
        if(playerTransform == null)
        {
            currentState= AIState.Patrol;
            return;
        }

        if (model.isAttacking) return;

        if(agent != null && agent.enabled)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    void HandleAttack(float distance)
    {
        if (!model.isAttacking)
        {
            FaceTarget();
            if (model.enemyType == EnemyType.EnemyRange)
                AttackRanged();
            if (model.enemyType == EnemyType.EnemyShort)
                AttackShort();
        }
    }
    #endregion
    void GoToNexPoint()
    {
        if (wayPoints.Length == 0) return;
       
            currentWaypoints = (currentWaypoints + 1) % wayPoints.Length;
            agent.SetDestination(wayPoints[currentWaypoints].position);
        anim.SetBool("isRunning", true);
        
    }
    void FaceTarget()
    {
        agent.updateRotation = false;
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
           StartCoroutine(ResetAttack());
        }
    }

    void AttackShort()
    {
        model.isAttacking = true;

        Sword meleePattern = GetComponent<Sword>();
        if (meleePattern != null)
        {
            meleePattern.PerformAttackShort();
            return;
        }
        // For short enemies, use distance to attack
        if (model.enemyType == EnemyType.EnemyShort)
        {
            // Use weapon point if available, otherwise default to a reasonable position
            Vector3 attackOrigin;
            if (model.weaponFirePoint != null)
            {
                attackOrigin = model.weaponFirePoint.position;
            }
            else
            {
                // Default to approximate weapon height
                attackOrigin = transform.position + transform.forward * 0.5f + Vector3.up * 1.0f;
            }

            Vector3 directionToPlayer = (active.Player.position - attackOrigin).normalized;
            float distanceToPlayer = Vector3.Distance(attackOrigin, active.Player.position);

            if (distanceToPlayer <= model.attackRange)
            {
                RaycastHit hit;

                // Debug visualization for melee attack
                Debug.DrawRay(attackOrigin, directionToPlayer * model.attackRange, Color.red, 0.5f);

                if (Physics.Raycast(attackOrigin, directionToPlayer, out hit, model.attackRange))
                {
                    // Check if we hit the player
                    if (hit.collider.CompareTag("Player"))
                    {
                        // Apply damage to player
                        if (hit.collider.gameObject.TryGetComponent<PlayerActive>(out var playerActive))
                        {
                            playerActive.TakeDamage(model.attackPower);
                            Debug.Log($"Melee attack hit player for {model.attackPower} damage");

                            // Create hit effect for player
                            if (Resources.Load<GameObject>("Prefabs/HitEffect"))
                            {
                                Instantiate(Resources.Load<GameObject>("Prefabs/HitEffect"), hit.point, Quaternion.LookRotation(hit.normal));
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Player hit but no PlayerHealth component found");
                        }
                    }
                }
            }

            // Also call enemyActive.AttackPlayer() to trigger the animation
            if (active != null)
            {
                active.AttackPlayer();
            }
        }

        StartCoroutine(ResetAttack()); 
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.5f);
        model.isAttacking = false;
    }

    public IEnumerator BulletTrailEffect(Vector3 targetPoint, Vector3 startPoint)
    {
        // Buat LineRenderer khusus untuk tembakan
        LineRenderer bulletTrail = gameObject.AddComponent<LineRenderer>();
        bulletTrail.startWidth = 0.05f;


        // Tunggu beberapoa saat
        yield return new WaitForSeconds(0.1f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.detectionRange);

        if (model == null) return;

        //Draw attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.attackRange);

        // Draw detection Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, model.detectionRange);
    }
}
