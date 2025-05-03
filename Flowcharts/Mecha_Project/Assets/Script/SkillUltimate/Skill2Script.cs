using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill2Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerActive.enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(playerData.skill2Damage);
            }
        }
    }
}
