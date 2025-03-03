using JetBrains.Annotations;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class EnemyActive : MonoBehaviour
{
    public GameObject PlayerObj;
    public Transform Player;
    public EnemyModel enemyData;
    public NavMeshAgent agent;
    public Animator anim;
    public GameMaster gameManager;
    [SerializeField] private CharacterController charController;
    [SerializeField] private CapsuleCollider deathCollider;
    public GameObject UIHealth;

    [Header("Atribut")]
    public float speed = 3.5f;
    public float stoppingDistance = 1.5f;

    //test
    private PlayerInput gameInput;
    public void Awake()
    {
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        Player = PlayerObj.GetComponent<Transform>();
        anim = GetComponent<Animator>();
        enemyData = GetComponent<EnemyModel>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
        deathCollider = GetComponent<CapsuleCollider>();
        charController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        UIHealth.SetActive(false);
        enemyData.health = enemyData.maxHealth;
        deathCollider.enabled = false;

        if(enemyData != null)
        {
            enemyData.Initialize(this);
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

        if (enemyData != null)
        {
            enemyData.isAttacking = true;
        }
    }

    public void OnAttackEnd()
    {
        if (enemyData != null)
        {
            enemyData.isAttacking = false;
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
        }
        UIHealthBar();
        //EnemyFollow();

        //test
        Damage();
    }
}
