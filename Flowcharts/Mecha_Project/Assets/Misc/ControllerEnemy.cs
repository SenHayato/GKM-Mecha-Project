using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ControllerEnemy : MonoBehaviour
{
    public float lookRadius = 10f;

    EnemyModel model;
    EnemyActive active;
    NavMeshAgent agent;
    Transform target;
    private void Start()
    {
        model = GetComponent<EnemyModel>();
        agent = GetComponent<NavMeshAgent>();
    }
}
