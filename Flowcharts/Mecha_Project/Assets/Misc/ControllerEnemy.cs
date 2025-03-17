using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControllerEnemy : MonoBehaviour
{
    [Header("Layering")]
    public LayerMask hitLayer;
    public LayerMask Player;
    public LayerMask obstacle;
    public LayerMask Ground;
    private EnemyActive enemyActive;
    private EnemyModel enemyModel;
    public Transform player;

    private Animator anim;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public NavMeshAgent navAgent;

    public bool alreadyAttacked;
    public GameObject projectile;


    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private Quaternion initialRotation;

    public Transform centrePoint; // Area central yang membuat enemy dapat bergerak bebas didalamnya
    void Awake()
    {
        player = GameObject.Find("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        if (!playerInSightRange && !playerInAttackRange) PatrolRange();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();
    }
    private IEnumerator CooldownPatrol()
    {
        yield return new WaitForSeconds(enemyModel.patrolWaitTime);
        PatrolRange();
    }
    private void PatrolRange()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            navAgent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Jika sudah mencapai point yang dituju maka
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    public void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
        {
            walkPointSet = true;
        }
    }

    public void Chase()
    {
        navAgent.SetDestination(player.position);
    }
    void Attack()
    {
        navAgent.SetDestination(transform.position);

        transform.LookAt(player);

        Debug.Log("Player Getting attacked by enemy");

        if (!alreadyAttacked)
        {
            // Menyimpan Rotasi awal
            initialRotation = transform.rotation;
            // Menghitung arah ke pemain
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Menggunakan LookAt untuk menghadap ke player
            transform.LookAt(player);

            CharacterController charcontrol = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<CharacterController>();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), enemyModel.attackCooldown);
        }
    }

    public void ResetAttack()
    {
        alreadyAttacked = false;
        initialRotation = transform.rotation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; ;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere (transform.position, sightRange);
    }
    void FieldOfView()
    {

    }
}
