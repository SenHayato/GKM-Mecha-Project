using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSmashScript : MonoBehaviour
{
    [SerializeField] BossActive bossActive;
    [SerializeField] int smashDamage;

    private void Start()
    {
        bossActive = FindFirstObjectByType<BossActive>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(smashDamage + bossActive.enemyModel.attackPower);
            }
        }
    }
}
