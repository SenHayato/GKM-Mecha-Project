using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEnemy : EnemyActive
{
    float attackTime = 0;
    [SerializeField] float nextAttackTime;
   

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
                //enemyModel.nextAttackTime = Time.time + enemyModel.attackCooldown;
                enemyModel.isAttacking = true;
            }
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    public override void PlayAnimation()
    {
        if (enemyModel.isAttacking)
        {
            anim.SetTrigger("Attack1");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
