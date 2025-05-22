using System.Collections;
using UnityEditor;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    public float shootRange;
    public float meleeRadius;
    public Transform rayCastPosition;
    public bool rifleShoot;
    public bool SecondState = false;
    public float chaseSpeed = 12f;

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
    [SerializeField] int attackChance = 0;

    public override void Attacking()
    {
        SecondStage();
        CheckPlayer();

        if (enemyModel.isGrounded)
        {
            AttackCooldown();
        }

        Debug.Log("Boss Attack");

        if (navAgent.enabled)
        {
            navAgent.SetDestination(player.position);
        }

        if (!hasAttacked)
        {
            anim.SetTrigger("StartAttack");
            //attackChance = Random.Range(0, 6);
            hasAttacked = true;
        }

        //Attack Generator
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
            if (enemyModel.attackCooldown <= 0)
            {
                //Debug.Log("Attack Chance 2"+ attackChance);
                int attackNum = Random.Range(0, 4);
                if (attackNum == 0)
                {
                    Invoke(nameof(GroundHit), preparingTime);
                }
                else if (attackNum == 1)
                {
                    Invoke(nameof(RammingAttack), preparingTime);
                }
                else if (attackNum == 2)
                {
                    Invoke(nameof(GroundSlash), preparingTime);
                }
                else //3
                {
                    Invoke(nameof(RammingAttack), preparingTime);
                }
            }
        }
        else //5
        {
            if (enemyModel.attackCooldown <= 0)
            {
                Invoke(nameof(GroundSlash), preparingTime);
            }
        }

        //Invoke(nameof(GroundSlash), preparingTime);
        //Invoke(nameof(SweepingAttack), preparingTime);
        //Invoke(nameof(UltimateAttack), preparingTime);

        //GroundHit
        GroundHitTeleport();
        //Ramming
        RammingAtPlayer();
        //GroundSlash
        GroundSlashStart();
    }

    void SecondStage()
    {
        if (enemyModel.health <= 500000)
        {
            SecondState = true;
            enemyModel.attackPower = 2500;
            navDefaultSpeed = 12f;
            chaseSpeed = 16f;
            //material berubah merah
        }
    }

    //Memakai state machine maka animation juga sebagai switch untuk mengaktifkan attack
    public override void PlayAnimation()
    {
        return;
    }

    #region Ultimate

    void UltimateAttack()
    {

    }

    #endregion

    #region Melee Attack

    public GameObject groundSmashCollider;
    public bool hasTeleported = false;

    #region GroundHit
    public void GroundHit()
    {
        if (enemyModel.attackCooldown <= 0)
        {
            enemyModel.isAttacking = true;
            anim.SetBool("GroundHit", true);
        }
    }

    public void GroundHitTeleport()
    {
        if (hasTeleported)
        {
            transform.position = player.position;
        }
    }

    public void GroundHitStateReset()
    {
        Invoke(nameof(GroundHitReset), 3f); //waktu disesuaikan dengan lama durasi attack
    }

    void GroundHitReset()
    {
        anim.SetBool("GroundHit",false);
    }

    #endregion

    #region GroundSlash

    public bool spawnSlash = false;
    [SerializeField] GameObject groundSlashObj;
    [SerializeField] Transform[] slashSpawner;

    public void GroundSlash()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= 20f)
        {
            enemyModel.isAttacking = true;
            anim.SetBool("GroundSlash", true);
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            navAgent.SetDestination(player.position);
            navAgent.speed = chaseSpeed;
        }
    }

    void GroundSlashStart()
    {
        if (spawnSlash)
        {
            if (!SecondState)
            {
                Instantiate(groundSlashObj, slashSpawner[0].transform.position, Quaternion.Euler(0f, slashSpawner[0].transform.eulerAngles.y, 0f));
            }
            else
            {
                foreach (Transform spawner in slashSpawner)
                {
                    Instantiate(groundSlashObj, spawner.transform.position, Quaternion.Euler(0f, spawner.transform.eulerAngles.y, 0f));
                }
            }
            spawnSlash = false;
        }
    }

    public void GroundSlashResetState()
    {
        Invoke(nameof(GroundSlashReset), 4f);
    }

    void GroundSlashReset()
    {
        anim.SetBool("GroundSlash", false);
    }


    #endregion

    #region RammingAttack

    public bool rammingLookAtPlayer = false;
    [SerializeField] float rammingDistance;
    public GameObject rammingCollider;

    public void RammingAttack()
    {
        if (enemyModel.attackCooldown <= 0)
        {
            enemyModel.isAttacking = true;
            anim.SetBool("RammingAttack", true);
        }
    }

    public void RammingAtPlayer()
    {
        if (rammingLookAtPlayer)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
                Debug.Log("Lihat Player");
            }
        }

        RammingStart();
    }

    void RammingStart()
    {
        if (rammingAttacking)
        {
            rammingCollider.SetActive(true);
            Vector3 direction = (player.position - transform.position).normalized;
            navAgent.SetDestination(transform.position + direction * rammingDistance);
            navAgent.speed = 20f;
        }
        else
        {
            rammingCollider.SetActive(false);
        }
    }

    public void RammingAttackResetState()
    {
        Invoke(nameof(RammingReset), 3f);
    }

    void RammingReset()
    {
        anim.SetBool("RammingAttack", false);
    }

    #endregion

    public void SweepingAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= 30f)
        {
            anim.SetTrigger("SweepingAttack");
        }
        else
        {
            navAgent.SetDestination(player.position);
            navAgent.speed = chaseSpeed;
        }
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
