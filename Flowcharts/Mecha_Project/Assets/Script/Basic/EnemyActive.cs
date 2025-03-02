using JetBrains.Annotations;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyActive : MonoBehaviour
{
    public GameObject PlayerObj;
    public Transform Player;
    public EnemyModel enemyData;
    public Animator anim;
    public GameMaster gameManager;
    [SerializeField] private CharacterController charController;
    [SerializeField] private CapsuleCollider deathCollider;
    public GameObject UIHealth;

    [Header("Atribut")]
    public float speed = 3.5f;
    public float stoppingDistance = 1.5f;

    // Audio and VFX references could be added here

    private PlayerInput gameInput;

    public void Awake()
    {
        // Find the player using GameObject.FindGameObjectWithTag
        PlayerObj = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObj != null)
        {
            Player = PlayerObj.transform;
            Debug.Log("Found player: " + PlayerObj.name);
        }
        else
        {
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator component not found on " + gameObject.name);
        }

        enemyData = GetComponent<EnemyModel>();
        if (enemyData == null)
        {
            Debug.LogError("EnemyModel component not found on " + gameObject.name);
        }

        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();

        deathCollider = GetComponent<CapsuleCollider>();
        if (deathCollider == null)
        {
            deathCollider = gameObject.AddComponent<CapsuleCollider>();
            deathCollider.center = new Vector3(0, 1, 0);
            deathCollider.height = 2f;
            deathCollider.radius = 0.5f;
            deathCollider.enabled = false;
        }

        charController = GetComponent<CharacterController>();
        if (charController == null)
        {
            Debug.LogWarning("CharacterController not found on " + gameObject.name);
            charController = gameObject.AddComponent<CharacterController>();
            charController.center = new Vector3(0, 1, 0);
            charController.height = 2f;
            charController.radius = 0.5f;
        }
    }

    private void Start()
    {
        if (UIHealth != null)
        {
            UIHealth.SetActive(false);
        }

        if (enemyData != null)
        {
            enemyData.health = enemyData.maxHealth;

            // Initialize enemy model with reference to this component
            enemyData.Initialize(this);
        }

        if (deathCollider != null)
        {
            deathCollider.enabled = false;
        }
    }

    public void UIHealthBar()
    {
        if (UIHealth != null && enemyData != null && enemyData.health < enemyData.maxHealth)
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

        // Play hit animation or sound here
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        // Check if enemy should die after taking damage
        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
        }
    }

    // Test function - could be removed in production
    public void Damage()
    {
        if (gameInput == null) return;

        InputAction inputAction = gameInput.actions.FindAction("TestKillEnemy");
        if (inputAction != null && inputAction.triggered)
        {
            TakeDamage(100);
            Debug.Log("Test kill enemy triggered");
        }
    }

    public void Death()
    {
        if (enemyData != null && enemyData.isDeath)
        {
            // Trigger death animation
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

            // Disable AI behavior
            if (enemyData != null)
            {
                enemyData.isMoving = false;
                enemyData.isAttacking = false;
            }

            Destroy(gameObject, 2f); // 2f is the duration of death animation
        }
    }

    public void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.KillCount++;
            Debug.Log("Kill count increased: " + gameManager.KillCount);
        }
    }

    // This Update only handles specific components related to this gameObject
    void Update()
    {
        Death();
        UIHealthBar();

        // Test function - could be removed in production
        Damage();
    }

    // Animation event handlers
    public void OnAttackStart()
    {
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

    // This method will be called by EnemyModel to apply movement
    public void ApplyMovement(Vector3 direction, float currentSpeed, bool shouldRotate)
    {
        if (charController != null && enemyData != null && !enemyData.isDeath)
        {
            // Apply movement using character controller
            charController.Move(currentSpeed * Time.deltaTime * direction);

            // Apply gravity
            if (!charController.isGrounded)
            {
                charController.Move(Vector3.down * 9.8f * Time.deltaTime);
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
}