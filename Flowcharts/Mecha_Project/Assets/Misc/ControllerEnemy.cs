using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControllerEnemy : MonoBehaviour
{
    public Transform player;
    EnemyModel model;
    EnemyActive active;

    NavMeshAgent agent;

    private void Awake()
    {
        agent= GetComponent<NavMeshAgent>();
        model = GetComponent<EnemyModel>();
    }

    private void Update()
    {
        agent.SetDestination(player.position);
    }

    private void Patrolling()
    {
        if (model.patrolRadius > 0)
        {

        }
    }

    private void WalkPoint()
    {

    }
}
