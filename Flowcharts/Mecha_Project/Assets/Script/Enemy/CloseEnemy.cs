using System.Collections;
using UnityEngine;

public class CloseEnemy : EnemyActive
{
    [SerializeField] float nextAttackTime;
    [SerializeField] private GameObject normalAttackCollider;
    [SerializeField] private GameObject heavyAttackCollider;

    [SerializeField] bool prepareAttack;

    public override void Attacking()
    {
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            enemyModel.isAttacking = true;
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    #region Animation Event

    // Panggil dari Animation Event
    public void EnableNormalAttack()
    {
        normalAttackCollider.SetActive(true);
    }

    public void DisableNormalAttack()
    {
        normalAttackCollider.SetActive(false);
    }

    public void EnableHeavyAttack()
    {
        heavyAttackCollider.SetActive(true);
    }

    public void DisableHeavyAttack()
    {
        heavyAttackCollider.SetActive(false);
    }

    int AttackNum;
    public void RandomAttackGen()
    {
        AttackNum = Random.Range(0, 6);
        anim.SetInteger("AttackIndex", AttackNum);
    }

    #endregion

    public override void PlayAnimationState()
    {
        //Attack
        if (enemyModel.isAttacking)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            anim.ResetTrigger("Attack");
        }
    }

    //void AttackAnim()
    //{
    //    anim.SetTrigger("Attack");
    //}
}
