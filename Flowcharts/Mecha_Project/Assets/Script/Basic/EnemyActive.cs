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
    //[SerializeField] CharacterController characterController; // Tidak digunakan, bisa dihapus
    public GameMaster gameManager;
    public GameObject UIHealth;
    public AudioSource hitSound;
    [SerializeField] CapsuleCollider hitCollider;
    [SerializeField] BoxCollider deathCollider;
    public Animator anim;

    [Header("Komponen Player")]
    private PlayerInput gameInput;

    [Header("Visual Effect")]
    [SerializeField] GameObject deathExplode;
    public GameObject stuntVFX;

    //flag
    public float navDefaultSpeed;
    public float beforeHitGround;
    public float defaultRotation;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyModel = GetComponent<EnemyModel>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        hitCollider = GetComponent<CapsuleCollider>();
        gameInput = FindObjectOfType<PlayerInput>(); // Lebih aman FindObjectOfType<PlayerInput>(); jika hanya ada satu
        gameManager = FindObjectOfType<GameMaster>(); // Lebih aman FindObjectOfType<GameMaster>(); jika hanya ada satu
    }

    private void Start()
    {
        stuntVFX.SetActive(false);
        defaultRotation = rotationSpeed;
        navDefaultSpeed = navAgent.speed;
        enemyModel.isGrounded = false; // Ini akan diatur oleh ApplyGravity()
        patrolPoints = GameObject.FindGameObjectsWithTag("EnemyWayPoint");
        hitSound = GetComponent<AudioSource>(); 
        
        UIHealth.SetActive(false);
        enemyModel.health = enemyModel.maxHealth;
        deathCollider.enabled = false;
        if (Physics.Raycast(transform.position, Vector3.down, beforeHitGround, groundLayer))
        {
            enemyModel.isGrounded = true;
            navAgent.enabled = true;
        } else {
            navAgent.enabled = false; // Nonaktifkan NavMeshAgent jika tidak di tanah saat Start
        }
    }

    void Update()
    {
        UIHealthBar(); 
        Damage(); 
        ApplyGravity();
        CheckingSight();
        GettingStunt();

        
        if (enemyModel.isDeath)
        {
            Death();
            return;
        }

        
        if (!enemyModel.isGrounded || enemyModel.isStunt)
        {
            if (navAgent.enabled) navAgent.SetDestination(transform.position); // Berhenti bergerak
            navAgent.enabled = false;
            PlayAnimation();
            return;
        }
        if (!navAgent.enabled) navAgent.enabled = true;

        if (playerInSight && playerInAttackRange)
        {
            anim.SetBool("Move", false);
            Attacking();
        }
        else if (playerInSight && !playerInAttackRange)
        {
            anim.SetBool("Move", true);
            ChasingPlayer();
        }
        else // Musuh Patrol jika kondisi diatas tidak terpenuhi
        {
            anim.SetBool("Move", true);
            if (enemyModel.isProvoke)
            {
                AlwaysChasing();
            }
            else
            {
                Patrolling();
            }
        }

        PlayAnimation();
    }


    void ApplyGravity()
    {
        enemyModel.isGrounded = Physics.Raycast(transform.position, Vector3.down, beforeHitGround, groundLayer);

        if (!enemyModel.isGrounded)
        {
            anim.SetBool("IsFalling", true);
            transform.position += gravityPower * Time.deltaTime * Vector3.down;
            if (navAgent.enabled) navAgent.enabled = false; 
        }
        else
        {
            anim.SetBool("IsFalling", false);
            if (!enemyModel.isStunt && !enemyModel.isDeath && !navAgent.enabled)
            {
                navAgent.enabled = true;
            }
        }
    }

    void GettingStunt()
    {
        if (enemyModel.isStunt)
        {
            if (navAgent.enabled)
            {
                navAgent.SetDestination(transform.position);
                navAgent.enabled = false;
            }
            anim.SetBool("IsStunt", true);
            stuntVFX.SetActive(true);
        }
        else
        {
            anim.SetBool("IsStunt", false);
            if (stuntVFX != null && stuntVFX.activeSelf) stuntVFX.SetActive(false);
            if (enemyModel.isGrounded && !navAgent.enabled && !enemyModel.isDeath)
            {
                navAgent.enabled = true;
            }
        }
    }

    #region Pengaturan
    void UIHealthBar()
    {
        if (enemyModel.health < enemyModel.maxHealth && enemyModel.health > enemyModel.minHealth)
        {
            UIHealth.SetActive(true);
        }
        else
        {
            UIHealth.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!enemyModel.isUnbeatable && !enemyModel.isDeath)
        {
            enemyModel.isProvoke = true;
            StartCoroutine(HitSound());
            enemyModel.isHit = true;
            
            enemyModel.health -= damage;
            Debug.Log(gameObject.name + " Kena Damage : " + damage.ToString());
            if (anim != null)
            {
                anim.SetTrigger("Hit");
            }
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
        if (inputAction != null && inputAction.triggered) // Cek inputAction tidak null juga
        {
            TakeDamage(100);
        }
    }

    public void Death()
    {
        enemyModel.health = enemyModel.minHealth;
        enemyModel.isDeath = true;

        // Hentikan pergerakan
        if (navAgent.enabled)
        {
            navAgent.SetDestination(transform.position);
            navAgent.speed = 0f;
            navAgent.enabled = false;
        }

        hitCollider.enabled = false;

        anim.SetBool("IsStunt", false);
        anim.SetBool("IsDeath", true);
        Debug.Log("Death Animation Triggered for " + gameObject.name);
        deathCollider.enabled = true; // Aktifkan death collider jika diperlukan
        Invoke(nameof(ExplodeVisual), 3.5f);
        Destroy(gameObject, 4f);
    }
    

    private bool exploded = false;
    void ExplodeVisual()
    {
        if (!exploded)
        {
            exploded = true;
            Instantiate(deathExplode, transform.position, Quaternion.identity);
        }
    }

    #endregion

    void CheckingSight()
    {
        if (player == null) return;

        playerInSight = Physics.CheckSphere(transform.position, enemyModel.sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, enemyModel.attackRange, playerLayer);
    }

    void Patrolling()
    {
        enemyModel.isPatrolling = true; 
        navAgent.speed = navDefaultSpeed;

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
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    void SearchWayPoint()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogWarning("No patrol points found for " + gameObject.name);
            walkPointSet = false;
            return;
        }

        int pointToWalk = Random.Range(0, patrolPoints.Length);
        walkPoint = patrolPoints[pointToWalk].transform.position;
        walkPointSet = true;
    }

    void ChasingPlayer()
    {
        enemyModel.isPatrolling = false; // Berhenti patrolling saat chasing
        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
    }

    void AlwaysChasing()
    {
        enemyModel.isPatrolling = false;
        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
    }

    public void ResetAttack()
    {
        enemyModel.isAttacking = false;
        Debug.Log("Reset Attack");
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

        Gizmos.color= Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * beforeHitGround);
    }
#endif


    private void OnDestroy()
    {
        gameManager.KillCount++;
    }

    public abstract void Attacking();
    public abstract void PlayAnimation();
}