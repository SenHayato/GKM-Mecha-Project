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
    public LayerMask hitLayer; //layer apa saja yang bisa dihit
    [SerializeField] LayerMask groundLayer; //layer yang bisa diinjak enemy

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] GameObject[] patrolPoints; //jika waypoint disediakan

    [Header("States")]
    public float rotationSpeed;
    public bool playerInSight;
    public bool playerInAttackRange;
    [SerializeField] float gravityPower;

    [Header("Komponen Enemy")]
    //[SerializeField] CharacterController characterController;
    public GameMaster gameManager;
    public GameObject UIHealth;
    public AudioSource hitSound;
    [SerializeField] CapsuleCollider hitCollider;
    [SerializeField] BoxCollider deathCollider;
    public Animator anim;

    [Header("Komponen Player")]
    private PlayerInput gameInput;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //player = GameObject.Find("CollisionPoint").transform;
        enemyModel = GetComponent<EnemyModel>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        //characterController = GetComponent<CharacterController>();
        hitCollider = GetComponent<CapsuleCollider>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        enemyModel.isGrounded = false;
        patrolPoints = GameObject.FindGameObjectsWithTag("EnemyWayPoint");
        hitSound = GetComponent<AudioSource>(); //hit sound
        UIHealth.SetActive(false);
        enemyModel.health = enemyModel.maxHealth;
        deathCollider.enabled = false;
        
    }

    void Update()
    {
        ApplyGravity();
        CheckingSight();

        if (!enemyModel.isDeath)
        {
            GettingStunt();
            if (enemyModel.isGrounded && !enemyModel.isStunt)
            {
                if (enemyModel.isProvoke)
                {
                    AlwaysChasing();
                }
                else
                {
                    if (!playerInSight && !playerInAttackRange)
                    {
                        Patrolling();
                    }

                    if (playerInSight && !playerInAttackRange)
                    {
                        ChasingPlayer();
                    }
                }

                if (playerInSight && playerInAttackRange)
                {
                    Attacking();
                }
            }
        }
        
        //StartCoroutine(HitSound());

        PlayAnimation();
        Death();
        UIHealthBar();
        Damage();
    }

    void ApplyGravity()
    {
        if (!enemyModel.isGrounded)
        {
            anim.SetBool("IsFalling", true);
            transform.position += gravityPower * Time.deltaTime * Vector3.down;
        }
        else
        {
            anim.SetBool("IsFalling", false);
            navAgent.enabled = true;
        }

        if (Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer))
        {
            enemyModel.isGrounded = true;
        }
    }

    void GettingStunt()
    {
        if (enemyModel.isStunt)
        {
            if (navAgent.enabled)
            {
                navAgent.SetDestination(transform.position);
            }
            anim.SetBool("IsStunt", true);
        }
        else
        {
            anim.SetBool("IsStunt", false);
        }
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
        StartCoroutine(HitSound());
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
        if (enemyModel.isHit && !enemyModel.wasHit)
        {
            enemyModel.wasHit = true;
            hitSound.Play();
        }
        yield return new WaitForSeconds(0.2f);
        enemyModel.wasHit = false;
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
            enemyModel.health = enemyModel.minHealth;
            enemyModel.isDeath = true;

            if (enemyModel.isDeath && enemyModel.isGrounded)
            {
                navAgent.speed = 0f;
                hitCollider.enabled = false;
                if (enemyModel != null && enemyModel.isDeath)
                {
                    if (anim != null)
                    {
                        //anim.SetTrigger("isDeath");
                        Debug.Log(" Death Animation Triggered ");
                    }
                    if (deathCollider != null)
                    {
                        deathCollider.enabled = true;
                    }

                    Destroy(gameObject, 7f); //lama animasi + effect ledakan
                }
            }
        }
    }

    #endregion

    void CheckingSight()
    {
        playerInSight = Physics.CheckSphere(transform.position, enemyModel.sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, enemyModel.attackRange, playerLayer);
    }

    void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWayPoint();
        }

        if (walkPointSet)
        {
            if (navAgent.enabled)
            {
                navAgent.SetDestination(walkPoint);
            }

            if (playerInAttackRange)
            {
                enemyModel.isPatrolling = false;
            }
            else
            {
                enemyModel.isPatrolling = true;
            }
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
        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
    }

    void AlwaysChasing()
    {
        if (enemyModel.isProvoke)
        {
            enemyModel.isPatrolling = false;
            if (navAgent.enabled)
            {
                navAgent.SetDestination(player.position);
            }
        }
    }

    public void ResetAttack()
    {
        enemyModel.isAttacking = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (enemyModel == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyModel.attackRange);
        UnityEditor.Handles.Label(transform.position + Vector3.forward * enemyModel.attackRange, "Attack Range");

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, enemyModel.sightRange);
        UnityEditor.Handles.Label(transform.position + Vector3.forward * enemyModel.sightRange, "Sight Range");
    }
#endif


    private void OnDestroy()
    {
        gameManager.KillCount++;
        //Effect meledak
    }

    public abstract void Attacking();
    public abstract void PlayAnimation();
}
