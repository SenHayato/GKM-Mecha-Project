using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEnemy : EnemyActive
{
    float attackTime = 0;
    [SerializeField] float nextAttackTime;
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private float weaponActiveTime = 0.5f; // durasi collider aktif


    public override void Attacking()
    {
        //attackTime += Time.deltaTime;
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            //float distance = Vector3.Distance(transform.position, player.position);
           
            //attackTime = 0f;
            Debug.Log("SwordAttack");
            //enemyModel.nextAttackTime = Time.time + enemyModel.attackCooldown;
            enemyModel.isAttacking = true;
            
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    private void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Invoke(nameof(DisableWeaponCollider), weaponActiveTime);
        }
    }

    private void DisableWeaponCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }

    public override void PlayAnimation()
    {
        //patrolling
        if (enemyModel.isPatrolling)
        {
            anim.SetFloat("Move", 1f);
        }
        else
        {
            anim.SetFloat("Move", 0f);
        }
        if (enemyModel.isAttacking)
        {
            anim.SetBool("Attack1", true);
        }
        else
        {
            anim.SetBool("Attack1", false);
            Debug.Log("ASDWA");
        }
        if (enemyModel.isDeath)
        {
            anim.SetBool("IsDeath", true);
        }
        else
        {
            anim.SetBool("IsDeath", false);
        }
    }
}
