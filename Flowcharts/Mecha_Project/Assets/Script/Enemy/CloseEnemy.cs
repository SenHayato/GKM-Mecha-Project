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
    }

    #endregion

    public override void PlayAnimation()
    {
        //Attack
        if (enemyModel.isAttacking)
        {
            StartCoroutine(AttackAnim());
        }
        else
        {
            StopCoroutine(AttackAnim());
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

    IEnumerator AttackAnim()
    {
        if (enemyModel.isAttacking)
        {
            anim.SetTrigger("Attack");
            Debug.Log("Attack ke " + AttackNum);
            anim.SetInteger("AttackIndex", AttackNum);

            yield return new WaitForSeconds(enemyModel.attackSpeed);
        }
    }
}
