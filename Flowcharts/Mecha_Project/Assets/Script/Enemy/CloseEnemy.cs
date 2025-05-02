using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEnemy : EnemyActive
{
    [Header("Komponen Enemy Range")]
    [SerializeField] Transform rayCastSpawn;
    float attackTime = 0;
    [SerializeField] Ray ray;
    [SerializeField] float nextAttackTime;
   
    [Header("RangeWeapon")]
    [SerializeField] Transform weaponMaxRange;

    public override void Attacking()
    {
        attackTime += Time.deltaTime;
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (attackTime >= nextAttackTime)
            {
                attackTime = 0f;
                Debug.Log("SwordAttack");
                anim.SetTrigger("Attack1");
                //enemyModel.nextAttackTime = Time.time + enemyModel.attackCooldown;
                enemyModel.isAttacking = true;
            }
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }
}
