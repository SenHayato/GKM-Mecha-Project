using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortWeapon : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;

    [Header("Visual Effect")]
    [SerializeField] GameObject slashEffect;
    [SerializeField] GameObject hitSlashEffect;

    private void Start()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPosition = other.transform.position;
        hitPosition.y = 1.2f;
        Instantiate(hitSlashEffect, hitPosition, Quaternion.identity);
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerActive>(out var player))
            {
                player.TakeDamage(enemyModel.attackPower);
                Debug.Log("SSS");
            }
        }
    }
}
