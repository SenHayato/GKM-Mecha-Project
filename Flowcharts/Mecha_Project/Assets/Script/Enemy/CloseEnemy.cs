using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloseEnemy : EnemyActive
{
    float attackTime = 0;
    [SerializeField] float nextAttackTime;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] float weaponActiveTime = 0.05f;


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

            //StartCoroutine(ActiveWeaponCollider());
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    //private IEnumerator ActiveWeaponCollider()
    //{
    //    weaponCollider.enabled = true;
    //    yield return new WaitForSeconds (weaponActiveTime);
    //    weaponCollider.enabled = false;
    //}// Panggil dari Animation Event
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    // Panggil dari Animation Event
    public void DisableWeaponCollider()
    {
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

        //Attack
        if (enemyModel.isAttacking)
        {
            StartCoroutine(AttackAnim());
            //anim.SetBool("Attack1", true);
        }
        else
        {
            StopCoroutine(AttackAnim());
            //anim.SetBool("Attack1", false);
            Debug.Log("ASDWA");
        }

        //Death
        if (enemyModel.isDeath)
        {
            anim.SetBool("IsDeath", true);
        }
        else
        {
            anim.SetBool("IsDeath", false);
        }
    }

    //void Attack()
    //{
    //    if (enemyModel.isAttacking)
    //    {
    //        int AttackNum = Random.Range(1, 4); //angka 4 atau max tidak termasuk maka ini hanya 3 gerakan
    //        Debug.Log("Attack ke " + AttackNum);
    //        anim.SetFloat("AttackIndex", AttackNum);
    //        anim.SetTrigger("Attack");

    //        yield return new WaitForSeconds(enemyModel.attackSpeed);
    //    }
    //}

    IEnumerator AttackAnim()
    {
        if (enemyModel.isAttacking)
        {
            int AttackNum = Random.Range(1, 4); //angka 4 atau max tidak termasuk maka ini hanya 3 gerakan
            Debug.Log("Attack ke " + AttackNum);
            anim.SetFloat("AttackIndex", AttackNum);
            anim.SetTrigger("Attack");

            yield return new WaitForSeconds(enemyModel.attackSpeed);
        }
    }
}
