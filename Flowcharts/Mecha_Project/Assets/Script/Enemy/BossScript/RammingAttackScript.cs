using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingAttackScript : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] int rammingDamage;

    void Start()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(enemyModel.attackPower + rammingDamage);
            }
        }
    }
}
