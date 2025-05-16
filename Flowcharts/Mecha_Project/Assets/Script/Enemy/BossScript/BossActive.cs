using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    [SerializeField] float shootRange;
    [SerializeField] float meleeRadius;
    [SerializeField] Transform rayCastPosition;

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
    [SerializeField] bool isBulletSpawn;
    [SerializeField] float attackRangeTime;
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
    [SerializeField] bool rangeAttacking1;
    [SerializeField] bool rangeAttacking2;
    [SerializeField] bool ultimating;

    public override void Attacking()
    {
        CheckPlayer();
        Debug.Log("Boss Attack");
        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }
        if (playerInRange)
        {
            StartCoroutine(FireRifle());
        }
    }

    private void CheckPlayer()
    {
        playerInMelee = Physics.CheckSphere(transform.position, meleeRadius, playerLayer);
        playerInRange = Physics.CheckSphere(transform.position, shootRange, playerLayer);
    }

    //Rifle
    IEnumerator FireRifle()
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
        rayCastPosition.forward = transform.forward;

        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        yield return new WaitForSeconds(timeBeforeAttack);
        if (angle < fireSlerpAngle)
        {
            if (!enemyModel.isAttacking)
            {
                enemyModel.isAttacking = true; //buat saklar doang
                Debug.Log("Enemy Menembak");
                StartCoroutine(RifleWeapon(direction));
                Invoke(nameof(ResetAttack), attackRangeTime);
            }
        }
    }

    IEnumerator RifleWeapon(Vector3 direction)
    {
        while (true)
        {
            Ray ray = new(rayCastPosition.position, direction);
            Vector3 targetPoint = ray.origin + 100f * shootRange * ray.direction;

            if (Physics.Raycast(ray, out RaycastHit hit, shootRange * 100f, hitLayer))
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
            StartCoroutine(BulletTrailRifle(targetPoint));
            Debug.DrawRay(rayCastPosition.position, direction * shootRange, Color.red, 1f);
            yield return new WaitForSeconds(rifleFireRate);
        }
    }

    private void FireGatling()
    {

    }

    IEnumerator BulletTrailRifle(Vector3 targetHit)
    {
        //muzzle kanan
        bulletRight.SetPosition(0, muzzleRight.position);
        bulletRight.SetPosition(1, targetHit);
        //muzzle kiri
        bulletLeft.SetPosition(0, muzzleLeft.position);
        bulletLeft.SetPosition(1, targetHit);

        //spawn peluru trail
        if (enemyModel.isAttacking && isBulletSpawn)
        {
            Debug.Log("BulletSpawn");
            bulletRight.enabled = true;
            bulletLeft.enabled = true;
            yield return new WaitForSeconds(rifleFireRate * 1.2f);
            bulletRight.enabled = false;
            bulletLeft.enabled= false;
            isBulletSpawn = false;
        }
    }
    
    public override void PlayAnimation()
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
