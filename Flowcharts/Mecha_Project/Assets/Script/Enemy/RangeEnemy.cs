using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : EnemyActive
{
    [Header("Komponen Enemy Range")]
    [SerializeField] Transform rayCastSpawn;
    [SerializeField] GameObject bulletHitEffect;
   
    [Header("RangeWeapon")]
    [SerializeField] Transform bulletSpawn;
    [SerializeField] LineRenderer bulletTrail;
    [SerializeField] bool isBulletSpawn = false;

    public override void Attacking()
    {
        navAgent.SetDestination(transform.position);
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (!enemyModel.isAttacking)
        {
            enemyModel.isAttacking = true;
            Debug.Log("EnemyTembak");
            isBulletSpawn = false;
            Vector3 targetPoint;
            if (Physics.Raycast(rayCastSpawn.position, rayCastSpawn.forward, out RaycastHit hit, enemyModel.attackRange, playerLayer))
            {
                targetPoint = hit.point;
                //Debug.Log(hit.point);
                //Debug.DrawRay(rayCastSpawn.position, rayCastSpawn.forward, Color.green);
                if (hit.collider.TryGetComponent<PlayerActive>(out var playerActive))
                {
                    playerActive.TakeDamage(enemyModel.attackPower);
                }
                Instantiate(bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
            Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
            StartCoroutine(BulletTrailEffect(hit.point));
        }
    }

    public override void PlayAnimation()
    {
        if (enemyModel.isAttacking)
        {
            anim.SetBool("IsAiming", true);
        }
        else
        {
            anim.SetBool("IsAiming", false);
        }
    }

    IEnumerator BulletTrailEffect(Vector3 targetHit)
    {
        bulletTrail.SetPosition(0, bulletSpawn.position);
        bulletTrail.SetPosition(1, targetHit);

        if (enemyModel.isAttacking && !isBulletSpawn)
        {
            bulletTrail.enabled = true;
            yield return new WaitForSeconds(0.05f);
            bulletTrail.enabled = false;
            isBulletSpawn = true;
        }
    }
}
