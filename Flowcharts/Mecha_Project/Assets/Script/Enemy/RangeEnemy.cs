using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : EnemyActive
{
    [Header("Komponen Enemy Range")]
    [SerializeField] Transform rayCastSpawn;
    [SerializeField] Ray ray;
   
    [Header("RangeWeapon")]
    [SerializeField] Transform weaponMaxRange;
    [SerializeField] Transform bulletSpawn;
    [SerializeField] LineRenderer bulletTrail;

    public override void Attacking()
    { 
        StartCoroutine(BulletTrailEffect());
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            Debug.Log("EnemyTembak");
            isBulletSpawn = false;
            enemyModel.isAttacking = true;
            if (Physics.Raycast(ray, out RaycastHit hit, enemyModel.attackRange, playerLayer))
            {

            }
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
        }
    }

    IEnumerator BulletTrailEffect()
    {
        bulletTrail.SetPosition(0, bulletSpawn.position);
        bulletTrail.SetPosition(1, weaponMaxRange.position);

        if (enemyModel.isAttacking && !isBulletSpawn)
        {
            bulletTrail.enabled = true;
            yield return new WaitForSeconds(0.05f);
            bulletTrail.enabled = false;
            isBulletSpawn = true;
        }
    }
}
