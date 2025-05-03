using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDmg : MonoBehaviour
{
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if (other.TryGetComponent<PlayerActive>(out var playerActive))
    //        {
    //            playerActive.TakeDamage(5000);
    //        }
    //    }
    //}
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Kena Damage Enemy");
            if (other.TryGetComponent<EnemyActive>(out var enemyActive))
            {
                Debug.Log("Enemy Damage " + 200);
                enemyActive.TakeDamage(200);
            }
        }
    }
}
