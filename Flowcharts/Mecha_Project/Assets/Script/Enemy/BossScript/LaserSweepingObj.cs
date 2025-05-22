using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSweepingObj : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] int sweepingDamage;

    void Start()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
        sweepingDamage += enemyModel.attackPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(sweepingDamage);
            }
        }
    }
}
