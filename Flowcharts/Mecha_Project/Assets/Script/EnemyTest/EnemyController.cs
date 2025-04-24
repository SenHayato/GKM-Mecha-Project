using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("EnemyProperties")]
    [SerializeField] EnemyData enemyData;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform player;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask groundLayer; //layer yang bisa diinjak enemy

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] bool walkPointSet;
    [SerializeField] float walkPointRange;
    [SerializeField] GameObject[] patrolPoints; //jika waypoint disediakan

    [Header("Attacking")]
    [SerializeField] float timeBetweenAttack;
    [SerializeField] bool wasAttack;

    [Header("States")]
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSight;
    [SerializeField] bool playerInAttackRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyData = GetComponent<EnemyData>();
        navAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        patrolPoints = GameObject.FindGameObjectsWithTag("EnemyWayPoint");
    }

    // Update is called once per frame
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
    }

    void CheckingSight()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
    }

    void Patrolling()
    {
        if (!walkPointSet)
        {
            //SearchWalkPoint();
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

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new(transform.position.x + randomX, transform.position.y + transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    void ChasingPlayer()
    {
        navAgent.SetDestination(player.position);
    }

    void Attacking()
    {
        navAgent.SetDestination(transform.position);
        transform.LookAt(player.position);

        if (!wasAttack)
        {
            wasAttack = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }
    }

    void ResetAttack()
    {
        wasAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
