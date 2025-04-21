using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyActive : MonoBehaviour
{
    public GameObject PlayerObj;
    public Transform Player;
    public EnemyModel enemyData;
    private AIController enemyAI;
    private ControllerEnemy AI;
    public NavMeshAgent agent;
    public Animator anim;
    //Components
    [SerializeField]
    private GameObject sword;
    [SerializeField]
    private GameObject swordOnLeg;

    public GameMaster gameManager;
    [SerializeField] private CharacterController charController;
    [SerializeField] private CapsuleCollider deathCollider;
    public GameObject UIHealth;

    [Header("Atribut")]
    public float speed = 3.5f;
    public float stoppingDistance = 1.5f;
    public AudioSource hitSound;
    public bool isHit;

    //flag
    bool wasHit = false;
    //test
    private PlayerInput gameInput;
    public void Awake()
    {
        //Player
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        Player = PlayerObj.GetComponent<Transform>();
        //Enemy
        enemyData = GetComponent<EnemyModel>();
        enemyAI = GetComponent<AIController>();
        //Enemy Model
        anim = GetComponent<Animator>();
        deathCollider = GetComponent<CapsuleCollider>();
        charController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        //Game Manager
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        hitSound = GetComponent<AudioSource>(); //hit sound
        UIHealth.SetActive(false);
        enemyData.health = enemyData.maxHealth;
        deathCollider.enabled = false;
        if(enemyAI != null)
        {
            enemyAI.Initialize(this);
        }

        deathCollider.enabled = false;
    }

   
    public void UIHealthBar()
    {
        if (enemyData.health < enemyData.maxHealth)
        {
            UIHealth.SetActive(true);
        }
    }

    public void TakeDamage(int damage)
    {
        isHit = true;
        StartCoroutine(HitSound());
        if (enemyData == null) return;

        enemyData.health -= damage;
        UIHealthBar();
        Debug.Log(gameObject.name + " Kena Damage " + damage.ToString());
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
        }
    }
    public void Equip()
    {
        float distanceToPlayer = Vector3.Distance(Player.position, transform.position);

        if (distanceToPlayer <= enemyData.alertRange)
        {
            enemyData.isEquipping = true;
            anim.SetTrigger("Equip");
        }
        else
        {
            enemyData.isEquipping = false;

        }
    }

    public void ActiveWeapon()
    {
        if (!enemyData.isEquipped)
        {
            sword.SetActive(true);
            swordOnLeg.SetActive(false);
            enemyData.isEquipped = !enemyData.isEquipped;
        }
        else
        {
            sword.SetActive(false);
            swordOnLeg.SetActive(true);
            enemyData.isEquipped = !enemyData.isEquipped;
        }
    }

    public void Equipped()
    {
        enemyData.isEquipping = false;
    }
    IEnumerator HitSound()
    {
        if (isHit && wasHit)
        {
            wasHit = false;
            hitSound.Play();
        }
        yield return new WaitForSeconds(0.5f);
        wasHit = true;
        isHit = false;
    }

    //test
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
                    anim.SetTrigger("Death");
                    Debug.Log("Death animation triggered");
                }

                if (deathCollider != null)
                {
                    deathCollider.enabled = true;
                }

                if (charController != null)
                {
                    charController.enabled = false;
                }

                if (enemyData != null)
                {
                    enemyData.isMoving = false;
                    enemyData.isAttacking = false;
                }
                Destroy(gameObject, 2f); // 2f is the duration of death animation
            }
        }
    }

    private void OnDestroy()
    {
        gameManager.KillCount++;
    }


    public void OnAttackEnd()
    {
        if (enemyData != null)
        {
            enemyData.isAttacking = false;
        }
    }

    public void AttackPlayer()
    {
        if (enemyData == null || enemyData.isDeath || Player == null) return;

        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        // Check if within attack range
        if (distanceToPlayer <= enemyData.attackRange && !enemyData.isAttacking)
        {
            // Face the player
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);

            // Trigger attack
            enemyData.isAttacking = true;
            if (anim != null)
            {
                anim.SetTrigger("Attack");
                anim.SetBool("IsAttacking", true);
            } 
        }
    }


    public void OnDrawGizmosSelected()
    {
        // Visualize attack range
        if (enemyData != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);

            // Visualize detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData.detectionRange);
        }
    }

    public void ApplyMovement(Vector3 direction, float currentSpeed, bool shouldRotate)
    {
        if (charController != null && enemyData != null && !enemyData.isDeath)
        {
            // Apply movement using character controller
            charController.Move(currentSpeed * Time.deltaTime * direction);

            // Apply gravity
            if (!charController.isGrounded)
            {
                charController.Move(9.8f * Time.deltaTime * Vector3.down);
            }

            // Handle rotation
            if (shouldRotate && direction != Vector3.zero)
            {
                // Ensure we only rotate around the y-axis
                Vector3 horizontalDirection = direction;
                horizontalDirection.y = 0;

                if (horizontalDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(horizontalDirection),
                        10f * Time.deltaTime
                    );
                }
            }

            // Update animation
            if (anim != null)
            {
                float moveSpeed = direction.magnitude > 0.1f ? 1f : 0f;
                anim.SetFloat("Move", moveSpeed);

                // Debug movement animation state
                if (direction.magnitude > 0.1f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    Debug.Log("Setting move animation: " + moveSpeed);
                }
            }
        }
    }

    void Update()
    {
        if (enemyData != null && enemyData.health <= enemyData.minHealth)
        {
            Death();
            Damage();
        }
        //For AIController handle behavior
        UIHealthBar();
    }
}
