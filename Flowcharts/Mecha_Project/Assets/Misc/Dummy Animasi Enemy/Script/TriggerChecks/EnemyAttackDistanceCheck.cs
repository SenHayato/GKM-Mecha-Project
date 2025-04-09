using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDistanceCheck : MonoBehaviour
{
    public GameObject MechaPlayer { get; set; }
    private Enemy _enemy;

    private void Awake()
    {
        MechaPlayer = GameObject.FindGameObjectWithTag("Player");

        _enemy = GetComponent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == MechaPlayer)
        {
            _enemy.SetAttackDistancebool(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == MechaPlayer)
        {
            _enemy.SetAttackDistancebool(false);
        }
    }
}
