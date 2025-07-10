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

    [Header("Bullet Fisik")]
    [SerializeField] GameObject bulletObj;
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

        Vector3 direction = (player.position - transform.position).normalized; //untuk muzzle
        Vector3 lookPlayer = (player.position - transform.position).normalized; //untuk lihat player

        // Random spread untuk miss tembakan
        float accuracyOffset = missChange; // makin besar makin meleset, untuk default 0.03f
        direction += new Vector3(Random.Range(-accuracyOffset, accuracyOffset), Random.Range(-accuracyOffset, accuracyOffset), 0f);
        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction); //untuk muzzle
        Quaternion lookAtPlayer = Quaternion.LookRotation(lookPlayer); //untuk lihat player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtPlayer, Time.deltaTime * rotationSpeed);
        rayCastSpawn.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        //rayCastSpawn.forward = transform.forward;

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        yield return new WaitForSeconds(timeBeforeAttack);
        if (angle < attackSlerpTollerance)
        {
            if (!enemyModel.isAttacking)
            {
                enemyModel.isAttacking = true; //buat saklar doang
                isBulletSpawn = true;
                Instantiate(bulletObj, rayCastSpawn.position, rayCastSpawn.rotation);
                //Debug.DrawRay(rayCastSpawn.position, direction * enemyModel.attackRange, Color.red, 1f);
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
            anim.SetBool("IsShooting", true);
        }
        else
        {
            anim.SetBool("IsShooting", false);
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

        ////patrolling
        //if (enemyModel.isPatrolling || enemyModel.isProvoke)
        //{
        //    anim.SetBool("Move", true);
        //}
        //else
        //{
        //    anim.SetBool("Move", false);
        //}
    }

    //IEnumerator BulletTrailEffect(Vector3 targetHit)
    //{
    //    bulletTrail.SetPosition(0, bulletSpawn.position);
    //    bulletTrail.SetPosition(1, targetHit);

    //    if (enemyModel.isAttacking && isBulletSpawn)
    //    {
    //        Debug.Log("BulletSpawn");
    //        bulletTrail.enabled = true;
    //        yield return new WaitForSeconds(0.05f);
    //        bulletTrail.enabled = false;
    //        isBulletSpawn = false;
    //    }
    //}
}
