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

    //[Header("Atribut")]
    //public float speed;   
    //public float stoppingDistance;

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
    }

    private void Start()
    {
        UIHealth.SetActive(false);
        enemyData.health = enemyData.maxHealth;
        deathCollider.enabled = false;
    }

    public void UIHealthBar()
    {
        if (enemyData.health < enemyData.maxHealth)
        {
            UIHealth.SetActive(true);
        }
    }

    //public void EnemyFollow()
    //{
    //    float distance = Vector3.Distance(transform.position, Player.position); // Periksa jarak

    //    if (distance > stoppingDistance)
    //    {
    //        Vector3 direction = (Player.position - transform.position).normalized; // Hitung arah menuju player
    //        transform.position += speed * Time.deltaTime * direction;
    //        transform.LookAt(Player); //Mengatur rotasi musuh untuk menghadap ke arah player
    //        anim.SetFloat("Move", 1f);
    //    }
    //    else
    //    {
    //        anim.SetFloat("Move", 0f);
    //    }
    //}

    public void TakeDamage(int damage) //test
    {
        enemyData.health -= damage;
        Debug.Log("Enemy Kena Damage " + damage.ToString());
    }

    //test
    public void Damage()
    {
        InputAction inputAction = gameInput.actions.FindAction("TestKillEnemy");
        InputAction testEnemy = inputAction;
        if (testEnemy.triggered)
        {
            enemyData.health -= 100;
        }
        if (enemyData.health <= enemyData.minHealth)
        {
            enemyData.isDeath = true;
        }
    }

    public void Death()
    {
        if (enemyData.isDeath)
        {
            deathCollider.enabled = true;
            charController.enabled = false;
            Destroy(gameObject, 2f); //2f adalah lama animasi kill
        }
    }

    public void OnDestroy()
    {
        gameManager.KillCount++;
    }

    void Update()
    {
        Death();
        UIHealthBar();
        //EnemyFollow();

        //test
        Damage();
    }
}
