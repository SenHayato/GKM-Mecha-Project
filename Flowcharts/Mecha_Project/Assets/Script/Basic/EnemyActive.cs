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
    public float speed;
    public float stoppingDistance;

    // Audio and VFX references could be added here

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
    }

    private void Start()
    {
        UIHealth.SetActive(false);
        enemyData.health = enemyData.maxHealth;
        deathCollider.enabled = false;

        // Register this component with the EnemyModel controller
        enemyData.Initialize(this);
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
        enemyData.health -= damage;
        UIHealthBar();
        Debug.Log("Enemy Kena Damage " + damage.ToString());

        // Play hit animation or sound here

        // Check if enemy should die after taking damage
        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
        }
    }

    // Test function - could be removed in production
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
        if (enemyData.isDeath)
        {
            // Trigger death animation
            if (anim != null)
            {
                anim.SetTrigger("Death");
            }

            deathCollider.enabled = true;
            charController.enabled = false;

            // Disable AI behavior
            enemyData.isMoving = false;
            enemyData.isAttacking = false;

            Destroy(gameObject, 2f); // 2f is the duration of death animation
        }
    }

    public void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.KillCount++;
        }
    }

    // This Update only handles specific components related to this gameObject
    // The AI logic is now in EnemyModel
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
        enemyData.isAttacking = true;
    }

    public void OnAttackEnd()
    {
        enemyData.isAttacking = false;
    }

    // This method will be called by EnemyModel to apply movement
    public void ApplyMovement(Vector3 direction, float currentSpeed, bool shouldRotate)
    {
        if (charController != null && !enemyData.isDeath)
        {
            // Apply movement using character controller
            charController.Move(currentSpeed * Time.deltaTime * direction);

            // Handle rotation
            if (shouldRotate && direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    10f * Time.deltaTime
                );
            }

            // Update animation
            if (anim != null)
            {
                anim.SetFloat("Move", direction.magnitude > 0.1f ? 1f : 0f);
            }
        }
    }
}