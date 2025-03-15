using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill1Script : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    HashSet<string> enemyTags;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void Start()
    {
        enemyTags = playerActive.enemyTags;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyTags.Contains(other.tag))
        {
            if (other.TryGetComponent<EnemyActive>(out var enemy))
            {
                enemy.TakeDamage(playerData.skill1Damage);
            }
        }
    }
}
