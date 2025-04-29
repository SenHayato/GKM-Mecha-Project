using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyActive : MonoBehaviour
{
    [Header("EnemyProperties")]
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] NavMeshAgent navAgent;
    public Transform player; //titik collision pada player, taruh di player
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask groundLayer; //layer yang bisa diinjak enemy

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] GameObject[] patrolPoints; //jika waypoint disediakan

    [Header("Attacking")]
    [SerializeField] bool isAttacking;
    [SerializeField] float rotationSpeed;

    [Header("States")]
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSight;
    [SerializeField] bool playerInAttackRange;

    [Header("RangeWeapon")]
    [SerializeField] Transform weaponMaxRange;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] LineRenderer bulletTrail;

    [Header("Komponen Enemy")]
    [SerializeField]
    private CharacterController characterController;
    public GameMaster gameManager;
    public GameObject UIHealth;
    public AudioSource hitSound;
    [SerializeField]
    private CapsuleCollider deathCollider;
    private Animator anim;

    [Header("Komponen Player")]
    private PlayerInput gameInput;


    //flag
    bool isBulletSpawn = false;

    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.Find("CollisionPoint").transform;
        enemyModel = GetComponent<EnemyModel>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        characterController = GetComponent<CharacterController>();
        deathCollider = GetComponent<CapsuleCollider>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        patrolPoints = GameObject.FindGameObjectsWithTag("EnemyWayPoint");
        hitSound = GetComponent<AudioSource>(); //hit sound
        UIHealth.SetActive(false);
        enemyModel.health = enemyModel.maxHealth;
        deathCollider.enabled = false;
        
    }


    void Update()
    {
        CheckingSight();

        if (!playerInSight && !playerInAttackRange)
        {
            Patrolling();
        }

        if (playerInSight && !playerInAttackRange)
        {
            ChasingPlayer();
        }

        if (playerInSight && playerInAttackRange)
        {
            Attacking();
        }
        StartCoroutine(BulletTrailEffect());

        Death();
        UIHealthBar();
        Damage();
    }
    #region Pengaturan
    void UIHealthBar()
    {
        if (enemyModel.health < enemyModel.maxHealth)
        {
            UIHealth.SetActive(true);
        }
    }
    public void TakeDamage(int damage)
    {
        enemyModel.isHit = true;
        if (enemyModel == null) return;

        enemyModel.health -= damage;
        UIHealthBar();
        Debug.Log(gameObject.name + " Kena Damage : " + damage.ToString());
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (enemyModel.health <= enemyModel.minHealth)
        {
            enemyModel.isDeath = true;
        }
    }
    IEnumerator HitSound()
    {
        if (enemyModel.isHit && enemyModel.wasHit)
        {
            enemyModel.wasHit = false;
            hitSound.Play();
        }
        yield return new WaitForSeconds(0.5f);
        enemyModel.wasHit = true;
        enemyModel.isHit = false;
    }
    public void Damage()
    {
        InputAction inputAction = gameInput.actions.FindAction("TestKillEnemy");
        InputAction testEnemy = inputAction;
        if (testEnemy.triggered)
        {
            TakeDamage(100);
        }
    }

    public void Death()
    {
        if (enemyModel.health <= enemyModel.minHealth)
        {
            enemyModel.isDeath = true;
            if (enemyModel != null && enemyModel.isDeath)
            {
                if (anim != null)
                {
                    anim.SetTrigger("isDeath");
                    Debug.Log(" Death Animation Triggered ");
                }
                if (deathCollider != null)
                {
                    deathCollider.enabled = true;
                }

                if (characterController != null)
                {
                    characterController.enabled = false;
                }

                if (enemyModel != null)
                {
                    enemyModel.isMoving = false;
                    enemyModel.isAttacking = false;
                }
                Destroy(gameObject, 2f);
            }
        }
    }

    #endregion

    void CheckingSight()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
    }

    void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWayPoint();
        }

        if (walkPointSet)
        {
            navAgent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWayPoint()
    {
        int pointToWalk = Random.Range(0, patrolPoints.Length);
        walkPoint = patrolPoints[pointToWalk].transform.position;
        walkPointSet = true;
    }

    void ChasingPlayer()
    {
        navAgent.SetDestination(player.position);
    }

    void Attacking()
    {
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!isAttacking)
        {
            //nembak raycast
            Debug.Log("EnemyTembak");
            isBulletSpawn = false;
            isAttacking = true;
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }


    IEnumerator BulletTrailEffect()
    {
        bulletTrail.SetPosition(0, bulletSpawn.position);
        bulletTrail.SetPosition(1, weaponMaxRange.position);

        if (isAttacking && !isBulletSpawn)
        {
            bulletTrail.enabled = true;
            yield return new WaitForSeconds(0.05f);
            bulletTrail.enabled = false;
            isBulletSpawn = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    private void OnDestroy()
    {
        gameManager.KillCount++;
        //Effect meledak
    }


    public void OnAttackEnd()
    {
        if (enemyModel != null)
        {
            enemyModel.isAttacking = false;
        }
    }

    //public void AttackPlayer()
    //{
    //    if (enemyData == null || enemyData.isDeath || Player == null) return;

    //    // Calculate distance to player
    //    float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

    //    // Check if within attack range
    //    if (distanceToPlayer <= enemyData.attackRange && !enemyData.isAttacking)
    //    {
    //        // Face the player
    //        Vector3 directionToPlayer = (Player.position - transform.position).normalized;
    //        directionToPlayer.y = 0;
    //        transform.rotation = Quaternion.LookRotation(directionToPlayer);

    //        // Trigger attack
    //        enemyData.isAttacking = true;
    //        if (anim != null)
    //        {
    //            anim.SetTrigger("Attack");
    //            anim.SetBool("IsAttacking", true);
    //        } 
    //    }
    //}


    //public void OnDrawGizmosSelected()
    //{
    //    // Visualize attack range
    //    if (enemyData != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);

    //        // Visualize detection range
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
    //    }
    //}

    //public void ApplyMovement(Vector3 direction, float currentSpeed, bool shouldRotate)
    //{
    //    if (charController != null && enemyData != null && !enemyData.isDeath)
    //    {
    //        // Apply movement using character controller
    //        charController.Move(currentSpeed * Time.deltaTime * direction);

    //        // Apply gravity
    //        if (!charController.isGrounded)
    //        {
    //            charController.Move(9.8f * Time.deltaTime * Vector3.down);
    //        }

    //        // Handle rotation
    //        if (shouldRotate && direction != Vector3.zero)
    //        {
    //            // Ensure we only rotate around the y-axis
    //            Vector3 horizontalDirection = direction;
    //            horizontalDirection.y = 0;

    //            if (horizontalDirection != Vector3.zero)
    //            {
    //                transform.rotation = Quaternion.Slerp(
    //                    transform.rotation,
    //                    Quaternion.LookRotation(horizontalDirection),
    //                    10f * Time.deltaTime
    //                );
    //            }
    //        }

    //        // Update animation
    //        if (anim != null)
    //        {
    //            float moveSpeed = direction.magnitude > 0.1f ? 1f : 0f;
    //            anim.SetFloat("Move", moveSpeed);

    //            // Debug movement animation state
    //            if (direction.magnitude > 0.1f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
    //            {
    //                Debug.Log("Setting move animation: " + moveSpeed);
    //            }
    //        }
    //    }
    //}
}
