using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BossActive : EnemyActive
{
    [Header("Boss Komponen")]
    public float shootRange;
    public float meleeRadius;
    //public Transform rayCastPosition;
    public bool rifleShoot;
    public bool SecondState = false;
    int SecondStageHealth;

    [Header("AttackState")]
    public bool readyToAttack = false;
    public bool playerInMelee;
    public bool playerInRange;
    public float preparingTime;
    public bool hasAttacked;

    [Header("Attack Generator")]
    [SerializeField] int attackNumber;

    [Header("Visual Effect")]
    [SerializeField] GameObject bulletHitEffect;

    public override void Attacking()
    {
        if (navAgent.enabled && !readyToAttack)
        {
            navAgent.stoppingDistance = 6f;
            navAgent.SetDestination(player.position);
        }

        //LockRotation();
        SecondStage();
        CheckPlayer();

        if (enemyModel.isGrounded)
        {
            AttackCooldown();

            if (enemyModel.attackCooldown <= 0 && !enemyModel.isAttacking)
            {
                anim.SetBool("Attacking", true);
                if (!wasAttackTriggered)
                {
                    wasAttackTriggered = true;
                    anim.SetTrigger("StartAttack");
                }
            }
        }
    }

    public void RandomRangeAttack()
    {
        if (!SecondState)
        {
            attackNumber = Random.Range(0, 7) + 1;
        }
        else
        {
            attackNumber = Random.Range(0, 8) + 1;
        }
        Debug.Log("Attack ke " + attackNumber);
        anim.SetInteger("AttackIndex", attackNumber);
    }

    //panggil pada animasi state
    public void StoppingMove()
    {
        if (navAgent.enabled)
        {
            navAgent.SetDestination(transform.position);
        }
    }

    public override void PlayAnimation()
    {
        return;
    }

    void SecondStage()
    {
        SecondStageHealth = enemyModel.maxHealth / 2;
        if (enemyModel.health <= SecondStageHealth)
        {
            SecondState = true;
            enemyModel.attackPower = 2500;
            //navDefaultSpeed = 8f;
            navAgent.speed = 8f;
        }
    }

    void AttackCooldown()
    {
        enemyModel.attackCooldown = Mathf.Max(0f, enemyModel.attackCooldown - Time.deltaTime);
    }

    public void SetAttackCooldown()
    {
        if (enemyModel.health >= 500000)
        {
            enemyModel.attackCooldown = 8f;
        }
        else
        {
            enemyModel.attackCooldown = 5f;
        }
    }

    private void CheckPlayer()
    {
        playerInMelee = Physics.CheckSphere(transform.position, meleeRadius, playerLayer);
        playerInRange = Physics.CheckSphere(transform.position, shootRange, playerLayer);
    }

    #region GatlingAttack dan RifleAttack------------------------------------------------------------------------------------

    [Header("Range Weapon Atribut")]
    [SerializeField] Transform[] muzzleWeapon;
    [SerializeField] LineRenderer[] bulletLaser;
    //[SerializeField] Transform rayCastSpawn;
    [SerializeField] float rangeRotationSpeed;
    public bool rifleAttacking;
    public bool gatlingAttacking;

    [Header("Attack Duration")]
    [SerializeField] float rifleAttackDuration;
    [SerializeField] float gatlingAttackDuration;

    [Header("Weapon")]
    [SerializeField] float rifleInterval;
    [SerializeField] float gatlingInterval;
    [SerializeField] float missChange;
    [SerializeField] float rayCastSpeedRot;
    [SerializeField] float rayOffsetForward;
    [SerializeField] float rayOffsetUp;

    //flag
    //bool isBulletSpawn;
    bool isFiring = false;

    //Rifle Shoot
    public void RifleAttackStart()
    {
        StartCoroutine(RifleAttack());
    }

    IEnumerator RifleAttack()
    {
        if (!rifleAttacking) yield break;

        if (!isFiring)
        {
            isFiring = true;
            StartCoroutine(RifleFire());
            Invoke(nameof(RangeReset), rifleAttackDuration);
        }

        while (rifleAttacking)
        {
            if (navAgent.enabled)
            {
                navAgent.SetDestination(transform.position);
            }
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rangeRotationSpeed);

            yield return null;
        }
    }

    IEnumerator RifleFire()
    {
        if (!rifleAttacking) yield break;

        while (rifleAttacking)
        {
            foreach (var rayCast in muzzleWeapon)
            {
                Vector3 direction = (player.position - rayCast.position).normalized;

                // tambahkan inaccuracy
                float accuracyOffset = missChange;
                direction += new Vector3(Random.Range(-accuracyOffset, accuracyOffset), Random.Range(-accuracyOffset, accuracyOffset) + rayOffsetUp, 0f);
                direction.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rayCast.rotation = Quaternion.RotateTowards(rayCast.rotation, targetRotation, rayCastSpeedRot * Time.deltaTime); // derajat per detik

                Debug.DrawRay(rayCast.position, rayCast.forward * shootRange, Color.blue, 1f);
                Ray ray = new(rayCast.position, rayCast.forward);
                Vector3 targetPoint = ray.origin + (shootRange + rayOffsetForward) * ray.direction;

                if (Physics.Raycast(ray, out RaycastHit hit, shootRange + rayOffsetForward, hitLayer))
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
                    targetPoint = ray.GetPoint(shootRange);
                }

                StartCoroutine(BulletTrail(targetPoint, rifleInterval));
                yield return new WaitForSeconds(rifleInterval);
            }
        }
    }

    IEnumerator BulletTrail(Vector3 targetPoint, float interval)
    {
        Debug.Log("BulletRifle");

        for (int i = 0; i < Mathf.Min(bulletLaser.Length, muzzleWeapon.Length); i++)
        {
            if (bulletLaser[i] == null || muzzleWeapon[i] == null) continue;

            bulletLaser[i].SetPosition(0, muzzleWeapon[i].position);
            bulletLaser[i].SetPosition(1, targetPoint);
            bulletLaser[i].enabled = true;
        }

        yield return new WaitForSeconds(interval);

        for (int i = 0; i < Mathf.Min(bulletLaser.Length, muzzleWeapon.Length); i++)
        {
            if (bulletLaser[i] != null)
                bulletLaser[i].enabled = false;
        }
    }

    private void RangeReset()
    {
        anim.SetBool("Attacking", false);
        isFiring = false;
        if (rifleAttacking)
        {
            rifleAttacking = false;
            StopCoroutine(RifleFire());
            StopCoroutine(RifleAttack());
        }

        if (gatlingAttacking)
        {
            gatlingAttacking = false;
            StopCoroutine(GatlingAttack());
            StopCoroutine(GatlingFire());
        }
    }

    //GatlingShoot
    public void GatlingAttackStart()
    {
        StartCoroutine(GatlingAttack());
    }

    IEnumerator GatlingAttack()
    {
        if (!gatlingAttacking) yield break;

        if (!isFiring)
        {
            isFiring = true;
            StartCoroutine(GatlingFire());
            Invoke(nameof(RangeReset), gatlingAttackDuration);
        }

        while (gatlingAttacking)
        {
            if (navAgent.enabled)
            {
                navAgent.SetDestination(transform.position);
            }
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rangeRotationSpeed);

            yield return null;
        }
    }

    IEnumerator GatlingFire()
    {
        if (!gatlingAttacking) yield break;

        while (gatlingAttacking)
        {
            foreach (var rayCast in muzzleWeapon)
            {
                Vector3 direction = (player.position - rayCast.position).normalized;

                // tambahkan inaccuracy
                float accuracyOffset = missChange;
                direction += new Vector3(Random.Range(-accuracyOffset, accuracyOffset), Random.Range(-accuracyOffset, accuracyOffset) + rayOffsetUp, 0f);
                direction.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rayCast.rotation = Quaternion.RotateTowards(rayCast.rotation, targetRotation, rayCastSpeedRot * Time.deltaTime); // derajat per detik

                Debug.DrawRay(rayCast.position, rayCast.forward * shootRange, Color.blue, 1f);
                Ray ray = new(rayCast.position, rayCast.forward);
                Vector3 targetPoint = ray.origin + (shootRange + rayOffsetForward) * ray.direction;

                if (Physics.Raycast(ray, out RaycastHit hit, shootRange + rayOffsetForward, hitLayer))
                {
                    targetPoint = hit.point;

                    if (hit.collider.TryGetComponent<PlayerActive>(out var playerActive))
                    {
                        playerActive.TakeDamage(enemyModel.attackPower);
                    }

                    Instantiate(bulletHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                }

                StartCoroutine(BulletTrail(targetPoint, gatlingInterval));
                yield return new WaitForSeconds(gatlingInterval);
            }
        }
    }

    #endregion
    #region MissileLaunch-------------------------------------------------------

    [Header("MissileAttack")]
    [SerializeField] GameObject bossMissileObj;
    [SerializeField] float missileInterval;
    [SerializeField] float missileDuration;

    public IEnumerator MissileAttacking()
    {
        float time = 0f;
        while (time < missileDuration)
        {
            Instantiate(bossMissileObj, player.transform.position, Quaternion.identity);
            float interval = 0f;
            while (interval < missileInterval)
            {
                interval += Time.deltaTime;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    #endregion

    #region GroundSlash---------------------------------------------------------------

    [Header("GroundSlash")]
    [SerializeField] Transform[] objSpawnPost;
    [SerializeField] GameObject groundSlashObj;
    public bool groundSlash = false;

    public void SpawningGroundSlash()
    {
        if (!SecondState)
        {
            Instantiate(groundSlashObj, objSpawnPost[0].position, objSpawnPost[0].rotation);
        }
        else
        {
            foreach(var spawner in objSpawnPost)
            {
                Instantiate(groundSlashObj, spawner.position, spawner.rotation);
            }
        }
    }

    #endregion

    #region SweepingAttack--------------------------------------------------------------

    [Header("SweepingLaser")]
    [SerializeField] GameObject[] sweepingLaser;

    public void SweepingLaserEnable()
    {
        foreach(var laser in sweepingLaser)
        {
            laser.SetActive(true);
        }
    }

    public void SweepingLaserDisable()
    {
        foreach(var laser in sweepingLaser)
        {
            laser.SetActive(false);
        }
    }
    #endregion

    #region RammingAttack-------------------------------------------------------------
    [Header("RammingAttack")]
    [SerializeField] GameObject rammingCollider;

    public void RammingEnable()
    {
        rammingCollider.SetActive(true);
    }

    public void RammingDisable()
    {
        rammingCollider.SetActive(false);
    }

    #endregion

    #region UltimateAttack-----------------------------------------------------------------------------
    [Header("UltimateAttack")]
    [SerializeField] GameObject LaserObj;

    public void UltimateEnable()
    {
        LaserObj.SetActive(true);
    }

    public void UltimateDisable()
    {
        LaserObj.SetActive(false);
    }
    #endregion

    private void OnDrawGizmos()
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
