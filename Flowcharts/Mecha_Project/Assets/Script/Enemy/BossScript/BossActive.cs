using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    public float shootRange;
    public float meleeRadius;
    public Transform rayCastPosition;
    public bool rifleShoot;
    public bool SecondState = false;

    [Header("AttackState")]
    public bool playerInMelee;
    public bool playerInRange;
    public float preparingTime;
    public bool hasAttacked;

    [Header("AttackMelee")]
    public GameObject meleeAttack1;
    public GameObject meleeAttack2;
    public GameObject meleeAttack3;
    public GameObject meleeAttack4;
    public GameObject ultimateObj;

    [Header("AttackRange")]
    public float missChange;
    public float fireSlerpAngle;
    public bool isBulletSpawn;
    public GameObject bulletHitEffect;

    [Header("RifleAttribut")]
    public float rifleFireRate;
    public float rifleAttackDuration;
    public Transform muzzleRight;
    public LineRenderer bulletRight;
    public Transform muzzleLeft;
    public LineRenderer bulletLeft;

    [Header("GatlingAttribut")]
    public float gatlingFireRate;
    public float gatlingAttackDuration;
    public Transform muzzleGatling;
    public LineRenderer bulletGatling;

    [Header("MissileBarrage")]
    [SerializeField] GameObject missileObj;
    [SerializeField] float missileDuration;
    [SerializeField] float missileInterval;

    [Header("AttackToggler")]
    [SerializeField] int attackNumber;
    public bool groundHitAttacking;
    public bool groundSlashAttacking;
    public bool rammingAttacking;
    public bool sweepingAttacking;
    public bool rifleAttacking;
    public bool gatlingAttacking;
    public bool missileAttacking;
    public bool ultimating;

    //flag
    private int attackChance = 0;

    public override void Attacking()
    {
        SecondStage();
        CheckPlayer();
        AttackCooldown();

        Debug.Log("Boss Attack");

        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }

        if (!hasAttacked)
        {
            attackChance = Random.Range(0, 6);
            anim.SetTrigger("StartAttack");
            hasAttacked = true;
        }

        if (attackChance <= 2) //0.1.2
        {
            //Debug.Log("Attack Chance 1"+ attackChance);
            int attackNum = Random.Range(0, 3);
            if (attackNum == 0)
            {
                Invoke(nameof(FireRifle), preparingTime);
            }
            else if (attackNum == 1)
            {
                Invoke(nameof(FireGatling), preparingTime);
            }
            else //2
            {
                Invoke(nameof(LaunchMissile), preparingTime);
            }
        }
        else if (attackChance >= 3 && attackChance <= 4) //3.4
        {
            //Debug.Log("Attack Chance 2"+ attackChance);
            int attackNum = Random.Range(0, 4);
            if (attackNum == 0)
            {

            }
            else if (attackNum == 1)
            {

            }
            else if (attackNum == 2)
            {

            }
            else //3
            {

            }
        }
        else //5
        {
            //Debug.Log("Attack Chance 3"+ attackChance);
        }

        //Invoke(nameof(GroundHit), preparingTime);
        //Invoke(nameof(GroundSlash), preparingTime);
        //Invoke(nameof(RammingAttack), preparingTime);
        //Invoke(nameof(SweepingAttack), preparingTime);
        //Invoke(nameof(UltimateAttack), preparingTime);

    }

    void SecondStage()
    {
        if (enemyModel.health <= 500000)
        {
            SecondState = true;
            navDefaultSpeed = 12f;
            //material berubah merah
        }
    }

    //Memakai state machine maka animation juga sebagai switch untuk mengaktifkan attack
    public override void PlayAnimation()
    {
        return;
    }

    void UltimateAttack()
    {

    }

    #region Melee Attack

    public void GroundHit()
    {
        anim.SetTrigger("GroundHit");
    }

    public void GroundSlash()
    {
        anim.SetTrigger("GroundSlash");
    }

    public void RammingAttack()
    {
        anim.SetTrigger("RammingAttack");
    }

    public void SweepingAttack()
    {
        anim.SetTrigger("SweepingAttack");
    }

    #endregion

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

    #region Rifle
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
                Invoke(nameof(RifleReset), rifleAttackDuration);
            }
        }
    }

    private void RifleReset()
    {
        rifleAttacking = false;
        anim.SetBool("RifleAttack", false);
    }

    IEnumerator WeaponRifle()
    {
        while (rifleAttacking)
        {
            Ray ray = new(rayCastPosition.position, rayCastPosition.forward);
            Vector3 targetPoint = ray.origin + 20f * shootRange * ray.direction;
            //Debug.DrawRay(ray.origin, ray.direction * shootRange * 100f, Color.blue);

            if (Physics.Raycast(ray, out RaycastHit hit, shootRange * 20f, hitLayer))
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

        Debug.Log("BulletRifle");
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

    #endregion

    #region GatlingGun
    private void FireGatling()
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
            //anim.SetTrigger("StartAttack");
            if (!enemyModel.isAttacking)
            {
                anim.SetBool("GatlingAttack", true);
                enemyModel.isAttacking = true;
                gatlingAttacking = true;
                StartCoroutine(WeaponGatling());

                //reset flag
                Invoke(nameof(ResetAttack), gatlingAttackDuration);
                Invoke(nameof(GatlingReset), gatlingAttackDuration);
            }
        }
    }

    void GatlingReset()
    {
        gatlingAttacking = false;
        anim.SetBool("GatlingAttack", false);
    }

    IEnumerator WeaponGatling()
    {
        while (gatlingAttacking)
        {
            Ray ray = new(rayCastPosition.position, rayCastPosition.forward);
            Vector3 targetPoint = ray.origin + 20f * shootRange * ray.direction;
            //Debug.DrawRay(ray.origin, ray.direction * shootRange * 100f, Color.blue);

            if (Physics.Raycast(ray, out RaycastHit hit, shootRange * 20f, hitLayer))
            {
                targetPoint = hit.point;
                if (hit.collider.TryGetComponent<PlayerActive>(out var playerActive))
                {
                    playerActive.TakeDamage(enemyModel.attackPower);
                }
                Instantiate(bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
            isBulletSpawn = true;
            StartCoroutine(BulletTrailGatling(targetPoint));
            yield return new WaitForSeconds(gatlingFireRate);
        }
    }

    IEnumerator BulletTrailGatling(Vector3 targetPoint)
    {
        Debug.Log("BulletGatling");
        bulletGatling.SetPosition(0, muzzleGatling.position);
        bulletGatling.SetPosition(1, targetPoint);

        //spawn peluru trail
        if (enemyModel.isAttacking && isBulletSpawn)
        {
            Debug.Log("BulletSpawn");
            bulletGatling.enabled = true;
            yield return new WaitForSeconds(rifleFireRate * 1.2f);
            bulletGatling.enabled = false;
            isBulletSpawn = false;
        }
    }

    #endregion

    #region MissileBarrage

    void LaunchMissile()
    {
        if (navAgent.enabled)
        {
            navAgent.SetDestination(transform.position);
        }

        if (enemyModel.attackCooldown <= 0)
        {
            //anim.SetTrigger("StartAttack");
            if (!enemyModel.isAttacking)
            {
                anim.ResetTrigger("StartAttack");
                anim.SetBool("MissileAttack", true);
                enemyModel.isAttacking = true;
                missileAttacking = true;
                StartCoroutine(MissileBarrage());

                //reset flag
                Invoke(nameof(ResetAttack), missileDuration);
                Invoke(nameof(ResetMissile), missileDuration);

            }
        }
    }

    IEnumerator MissileBarrage()
    {
        while (missileAttacking)
        {
            Instantiate(missileObj, player.position, Quaternion.identity);
            yield return new WaitForSeconds(missileInterval);
        }
    }

    private void ResetMissile()
    {
        missileAttacking = false;
        anim.SetBool("MissileAttack", false);
    }

    #endregion
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
