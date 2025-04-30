using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : EnemyActive
{
    [Header("Komponen Enemy Range")]
    public float test;
    public override void Attacking()
    {
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            Debug.Log("EnemyTembak");
            isBulletSpawn = false;
            enemyModel.isAttacking = true;
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }
}
