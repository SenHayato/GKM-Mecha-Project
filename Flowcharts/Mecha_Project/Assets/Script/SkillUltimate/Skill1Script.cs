using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill1Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string[] enemyTags = { "Enemy", "MiniBoss", "Boss" };
        if (enemyTags.Contains(other.tag))
        {
            if (other.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(playerData.skill1Damage);
            }
        }
    }
}
