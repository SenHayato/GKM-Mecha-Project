using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    [SerializeField] float shootRange;
    [SerializeField] float meleeRadius;
    [SerializeField] Transform rayCastPosition;
    [SerializeField] bool rifleShoot;

    [SerializeField] bool playerInMelee;
    [SerializeField] bool playerInRange;

    [Header("AttackMelee")]
    [SerializeField] GameObject meleeAttack1;
    [SerializeField] GameObject meleeAttack2;
    [SerializeField] GameObject meleeAttack3;
    [SerializeField] GameObject meleeAttack4;
    [SerializeField] GameObject ultimateObj;

    [Header("AttackRange")]
    [SerializeField] float missChange;
    [SerializeField] float fireSlerpAngle;
    [SerializeField] float timeBeforeAttack; //preparing sebelum menembak
    [SerializeField] float rifleFireRate;
    [SerializeField] float attackRangeTime;
    [SerializeField] float rifleAttackDuration;
    [SerializeField] float gatlingAttackTime;
    [SerializeField] bool isBulletSpawn;
    [SerializeField] GameObject bulletHitEffect;

    [Header("Bullet Trail Dual Rifle")]
    [SerializeField] Transform muzzleRight;
    [SerializeField] LineRenderer bulletRight;
    [SerializeField] Transform muzzleLeft;
    [SerializeField] LineRenderer bulletLeft;

    [Header("AttackToggler")]
    [SerializeField] bool meleeAttacking1;
    [SerializeField] bool meleeAttacking2;
    [SerializeField] bool meleeAttacking3;
    [SerializeField] bool meleeAttacking4;
    [SerializeField] bool rifleAttacking;
    [SerializeField] bool gatlingAttacking;
    [SerializeField] bool ultimating;

    public override void Attacking()
    {
        CheckPlayer();
        AttackCooldown();
        Debug.Log("Boss Attack");
        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
        if (playerInRange)
        {
            //StartCoroutine(FireRifle());
            FireRifle();
        }
    }

    public override void PlayAnimation()
    {

    }

    void AttackCooldown()
    {
        if (!enemyModel.isAttacking)
        {
            enemyModel.attackCooldown = Mathf.Max(0f, enemyModel.attackCooldown - Time.deltaTime);
        }
        else
        {
            if (enemyModel.health >= 500000)
            {
                enemyModel.attackCooldown = 8f;
            }
            else
            {
                enemyModel.attackCooldown = 3f;
            }
        }
    }

    private void CheckPlayer()
    {
        playerInMelee = Physics.CheckSphere(transform.position, meleeRadius, playerLayer);
        playerInRange = Physics.CheckSphere(transform.position, shootRange, playerLayer);
    }

    //Rifle
    void FireRifle()
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
        //rayCastPosition.forward = transform.forward;

        if (enemyModel.attackCooldown <= 0)
        {
            if (!enemyModel.isAttacking)
            {
                enemyModel.isAttacking = true;
                rifleAttacking = true;
                StartCoroutine(WeaponRifle());

                //reset flag
                Invoke(nameof(ResetAttack), rifleAttackDuration);
                Invoke(nameof(WeaponReset), rifleAttackDuration);
            }
        }
    }

    private void WeaponReset()
    {
        rifleAttacking = false;
    }

    IEnumerator WeaponRifle()
    {
        while (rifleAttacking)
        {
            Ray ray = new(rayCastPosition.position, rayCastPosition.forward);
            Vector3 targetPoint = ray.origin + 100f * shootRange * ray.direction;
            //Debug.DrawRay(ray.origin, ray.direction * shootRange * 100f, Color.blue);

            if (Physics.Raycast(ray, out RaycastHit hit, shootRange * 100f, hitLayer))
            {
                targetPoint = hit.point;
                if (hit.collider.TryGetComponent<PlayerActive>(out var playerActive))
                {
                    playerActive.TakeDamage(enemyModel.attackPower);
                }
                Instantiate(bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
            isBulletSpawn = true;
            StartCoroutine(BulletTrailRifle(targetPoint));
            yield return new WaitForSeconds(rifleFireRate);
        }

    }

    IEnumerator BulletTrailRifle(Vector3 targetPoint)
    {
        //muzzle kanan
        bulletRight.SetPosition(0, muzzleRight.position);
        bulletRight.SetPosition(1, targetPoint);
        //muzzle kiri
        bulletLeft.SetPosition(0, muzzleLeft.position);
        bulletLeft.SetPosition(1, targetPoint);

        //spawn peluru trail
        if (enemyModel.isAttacking && isBulletSpawn)
        {
            Debug.Log("BulletSpawn");
            bulletRight.enabled = true;
            bulletLeft.enabled = true;
            yield return new WaitForSeconds(rifleFireRate * 1.2f);
            bulletRight.enabled = false;
            bulletLeft.enabled = false;
            isBulletSpawn = false;
        }

    }

    //Gattling
    private void FireGatling()
    {

    }
    private void OnDrawGizmosSelected()
    {
        if (enemyModel == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        UnityEditor.Handles.Label(transform.position + Vector3.forward * shootRange, "Shoot Range");

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, meleeRadius);
        UnityEditor.Handles.Label(transform.position + Vector3.forward * meleeRadius, "Melee Range");
    }
}
