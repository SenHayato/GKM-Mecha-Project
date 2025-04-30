using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public abstract class EnemyActive : MonoBehaviour
{
    [Header("EnemyProperties")]
    public EnemyModel enemyModel;
    public NavMeshAgent navAgent;
    public Transform player; //titik collision pada player, taruh di player
    public LayerMask playerLayer;
    [SerializeField] LayerMask groundLayer; //layer yang bisa diinjak enemy

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] GameObject[] patrolPoints; //jika waypoint disediakan

    [Header("States")]
    public float rotationSpeed;
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSight;
    [SerializeField] bool playerInAttackRange;

    [Header("Komponen Enemy")]
    [SerializeField] CharacterController characterController;
    public GameMaster gameManager;
    public GameObject UIHealth;
    public AudioSource hitSound;
    [SerializeField] CapsuleCollider deathCollider;
    [SerializeField] Animator anim;

    [Header("Komponen Player")]
    private PlayerInput gameInput;

    //flag
    public bool isBulletSpawn = false;

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
        StartCoroutine(HitSound());

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


    public void ResetAttack()
    {
        enemyModel.isAttacking = false;
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

    public abstract void Attacking();
}
