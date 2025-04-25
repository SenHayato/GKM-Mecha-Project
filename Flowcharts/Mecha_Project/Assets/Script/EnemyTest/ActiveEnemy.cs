using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveEnemy : MonoBehaviour
{
    [Header ("Enemy Script")]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private EnemyData  data;
    [SerializeField]
    private EnemyController controller;

    [Header("Komponen Enemy")]
    [SerializeField]
    private CharacterController characterController;
    public GameMaster gameManager;
    public GameObject UIHealth;
    [SerializeField]
    private CapsuleCollider deathCollider;

    [Header("Komponen Player")]
    private PlayerInput gameInput;
   


    private void Awake()
    {
        anim = GetComponent<Animator>();
        data = GetComponent<EnemyData>();
        controller = GetComponent<EnemyController>();
        characterController = GetComponent<CharacterController>();
        gameManager = GetComponent<GameMaster>();
        deathCollider = GetComponent<CapsuleCollider>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        gameManager = FindAnyObjectByType<GameMaster>();
    }

    private void Start()
    {
        UIHealth.SetActive(false);
        deathCollider.enabled = false;

    }
    public void UIHealthBar()
    {
        if (data.health < data.maxHealth)
        {
            UIHealth.SetActive(true);
        }
    }
    public void TakeDamage(int damage)
    {
        data.isHit = true;
        if (data == null) return;

        data.health -= damage;
        UIHealthBar();
        Debug.Log(gameObject.name + " Kena Damage : " + damage.ToString());
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (data.health <= data.minHealth)
        {
            data.isDeath = true;
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
        if (data.health <= data.minHealth)
        {
            data.isDeath = true;
            if(data != null && data.isDeath)
            {
                if(anim != null)
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
                    controller.enabled = false;
                }

                if(data != null)
                {
                    data.isMoving = false;
                    data.isAttacking = false;
                }
                Destroy(gameObject, 2f);
            }
        }
    }

    private void OnDestroy()
    {
        gameManager.KillCount++;
        //Effect meledak
    }
    private void Update()
    {
        if(data != null && data.health <= data.minHealth){
            Death();
            Damage();
        }
        UIHealthBar();
    }
}
