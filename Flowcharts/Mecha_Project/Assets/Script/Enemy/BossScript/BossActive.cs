using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    public float shootRange;
    public float meleeRadius;
    public Transform rayCastPosition;
    public bool rifleShoot;

    public bool playerInMelee;
    public bool playerInRange;

    [Header("AttackMelee")]
    public GameObject meleeAttack1;
    public GameObject meleeAttack2;
    public GameObject meleeAttack3;
    public GameObject meleeAttack4;
    public GameObject ultimateObj;

    [Header("AttackRange")]
    public float missChange;
    public float fireSlerpAngle;
    public float timeBeforeAttack; //preparing sebelum menembak
    public float rifleFireRate;
    public float attackRangeTime;
    public float rifleAttackDuration;
    public float gatlingAttackTime;
    public bool isBulletSpawn;
    public GameObject bulletHitEffect;

    [Header("Bullet Trail Dual Rifle")]
    public Transform muzzleRight;
    public LineRenderer bulletRight;
    public Transform muzzleLeft;
    public LineRenderer bulletLeft;

    [Header("AttackToggler")]
    public bool meleeAttacking1;
    public bool meleeAttacking2;
    public bool meleeAttacking3;
    public bool meleeAttacking4;
    public bool rifleAttacking;
    public bool gatlingAttacking;
    public bool ultimating;

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
           //FireRifle();
           anim.SetTrigger("StartAttack");
           Invoke(nameof(FireRifle), 2f);
        }

        //int AttackNum = Random.Range(0, 5);
        //if (AttackNum <= 3)
        //{
        //    //range attack
        //}
        //else
        //{
        //    //melee attack
        //}
    }

    //Memakai state machine maka animation juga sebagai switch untuk mengaktifkan attack
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
    public void FireRifle()
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
        Debug.Log("Arah " + direction);
        if (enemyModel.attackCooldown <= 0)
        {
            //anim.SetTrigger("StartAttack");
            if (!enemyModel.isAttacking)
            {
                anim.SetBool("RifleAttack", true);
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
        anim.SetBool("RifleAttack", false);
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

        Debug.Log("Bullet");
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
