using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    [Header("EnemyProperties")]
    [SerializeField] EnemyData enemyData;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform player; //titik collision pada player, taruh di player
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask groundLayer; //layer yang bisa diinjak enemy

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] GameObject[] patrolPoints; //jika waypoint disediakan

    [Header("Attacking")]
    [SerializeField] float timeBetweenAttack;
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
        enemyData = GetComponent<EnemyData>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        characterController = GetComponent<CharacterController>();
        gameManager = GetComponent<GameMaster>();
        deathCollider = GetComponent<CapsuleCollider>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
    }
    void Start()
    {
        patrolPoints = GameObject.FindGameObjectsWithTag("EnemyWayPoint");
        deathCollider.enabled = false;
    }

    // Update is called once per frame
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
    void UIHealthBar()
    {
        if (enemyData.health < enemyData.maxHealth)
        {
            UIHealth.SetActive(true);
        }
    }
    public void TakeDamage(int damage)
    {
        enemyData.isHit = true;
        if (enemyData == null) return;

        enemyData.health -= damage;
        UIHealthBar();
        Debug.Log(gameObject.name + " Kena Damage : " + damage.ToString());
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
        }
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
        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
            if (enemyData != null && enemyData.isDeath)
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

                if (enemyData != null)
                {
                    enemyData.isMoving = false;
                    enemyData.isAttacking = false;
                }
                Destroy(gameObject, 2f);
            }
        }
    }

    

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
        //transform.LookAt(player.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!isAttacking)
        {
            //nembak raycast
            Debug.Log("EnemyTembak");
            isBulletSpawn = false;
            isAttacking = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
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
}
