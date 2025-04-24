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
        enemyData = GetComponent<EnemyData>();
        navAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
