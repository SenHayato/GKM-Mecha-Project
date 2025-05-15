using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemy : EnemyActive
{
    [Header("Komponen Enemy Range")]
    [SerializeField] Transform rayCastSpawn;
    [SerializeField] GameObject bulletHitEffect;
    [SerializeField] float missChange;
    [SerializeField] float attackSlerpTollerance;
    [SerializeField] float timeBeforeAttack;
    //private Ray ray;
   
    [Header("RangeWeapon")]
    [SerializeField] Transform bulletSpawn;
    [SerializeField] LineRenderer bulletTrail;
    [SerializeField] bool isBulletSpawn = false;

    public override void Attacking()
    {
        StartCoroutine(AttackFire());
    }

    IEnumerator AttackFire()
    {
        if (navAgent.enabled)
        {
            navAgent.SetDestination(transform.position);
        }

        Vector3 direction = (player.position - transform.position).normalized;

        // Random spread untuk miss tembakan
        float accuracyOffset = missChange; // makin besar makin meleset, untuk default 0.03f
        direction += new Vector3(Random.Range(-accuracyOffset, accuracyOffset), Random.Range(-accuracyOffset, accuracyOffset), 0f);
        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rayCastSpawn.forward = transform.forward;

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        yield return new WaitForSeconds(timeBeforeAttack);
        if (angle < attackSlerpTollerance)
        {
            if (!enemyModel.isAttacking)
            {
                enemyModel.isAttacking = true;
                Debug.Log("Enemy Menembak");
                Ray ray = new(rayCastSpawn.position, direction);
                Vector3 targetPoint = ray.origin + 100f * enemyModel.attackRange * ray.direction;

                if (Physics.Raycast(ray, out RaycastHit hit, enemyModel.attackRange * 100f, hitLayer))
                {
                    targetPoint = hit.point;
                    if (hit.collider.TryGetComponent<PlayerActive>(out var playerActive))
                    {
                        playerActive.TakeDamage(enemyModel.attackPower);
                    }
                    Instantiate(bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }
                else
                {
                    Debug.Log("Tembakan Musuh Meleset");
                }

                isBulletSpawn = true;
                StartCoroutine(BulletTrailEffect(targetPoint));
                Debug.DrawRay(rayCastSpawn.position, direction * enemyModel.attackRange, Color.red, 1f);
                Invoke(nameof(ResetAttack), enemyModel.attackSpeed);
            }
        }
    }

    public override void PlayAnimation()
    {
        if (enemyModel.isDeath)
        {
            anim.SetBool("IsDeath", true);
        }
        else
        {
            anim.SetBool("IsDeath", false);
        }

        //aiming
        if (playerInAttackRange && enemyModel.isGrounded)
        {
            anim.SetBool("IsAiming", true);
        }
        else
        {
            anim.SetBool("IsAiming", false);
        }

        //attacking
        //if (enemyModel.isAttacking)
        //{
        //    anim.SetBool("IsShooting", true);
        //}
        //else
        //{
        //    anim.SetBool("IsShooting", false);
        //}

        //patrolling
        if (enemyModel.isPatrolling || enemyModel.isProvoke)
        {
            anim.SetFloat("Move", 1f);
        }
        else
        {
            anim.SetFloat("Move", 0f);
        }
    }

    IEnumerator BulletTrailEffect(Vector3 targetHit)
    {
        bulletTrail.SetPosition(0, bulletSpawn.position);
        bulletTrail.SetPosition(1, targetHit);

        if (enemyModel.isAttacking && isBulletSpawn)
        {
            Debug.Log("BulletSpawn");
            bulletTrail.enabled = true;
            yield return new WaitForSeconds(0.05f);
            bulletTrail.enabled = false;
            isBulletSpawn = false;
        }
    }
}
