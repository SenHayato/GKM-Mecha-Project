using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RammingAttackScript : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] int rammingDamage;
    [SerializeField] GameObject hitEffect;

    void Awake()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPost = other.transform.position;
        hitPost.y = 1.2f;
        Instantiate(hitEffect, hitPost, Quaternion.identity);
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var playerActive))
            {
                playerActive.TakeDamage(enemyModel.attackPower + rammingDamage);
            }
        }
    }
}
