using System.Collections;
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

    //flag
    bool stayPosition = false;

    public override void Attacking()
    {
        if (navAgent.enabled && !readyToAttack && !stayPosition)
        {
            //navAgent.stoppingDistance = 6f;
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
                    RandomRangeAttack();
                }
            }
        }
    }

    public void RandomRangeAttack()
    {
        int random = Random.Range(0, 100);

        if (!SecondState)
        {
            if (random < 20) attackNumber = 1;            //1-19
            else if (random < 50) attackNumber = 2;
            else if (random < 80) attackNumber = 3;
            else attackNumber = 4;                        //81-99
        }
        else
        {
            //attack 4-8
            if (random < 20) attackNumber = 4;            //1-19
            else if (random < 40) attackNumber = 5;
            else if (random < 60) attackNumber = 6;
            else if (random < 80) attackNumber = 7;
            else attackNumber = 8;
        }
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
            //navAgent.speed = 8f;
        }
    }

    void AttackCooldown()
    {
        enemyModel.attackCooldown = Mathf.Max(0f, enemyModel.attackCooldown - Time.deltaTime);
    }

    public void SetAttackCooldown()
    {
        if (!SecondState)
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
    [SerializeField] AudioSource[] bulletSounds;
    //[SerializeField] Transform rayCastSpawn;
    [SerializeField] float rangeRotationSpeed;
    public bool rifleAttacking = false;
    public bool gatlingAttacking = false;

    [Header("Range Attack Duration")]
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

                StartCoroutine(BulletTrail(targetPoint, rifleInterval));
                yield return new WaitForSeconds(rifleInterval);
            }
        }
    }

    IEnumerator BulletTrail(Vector3 targetPoint, float interval)
    {
        for (int i = 0; i < Mathf.Min(bulletLaser.Length, muzzleWeapon.Length); i++)
        {
            if (bulletLaser[i] == null || muzzleWeapon[i] == null) continue;

            bulletLaser[i].SetPosition(0, muzzleWeapon[i].position);
            bulletLaser[i].SetPosition(1, targetPoint);
            bulletLaser[i].enabled = true;
            bulletSounds[i].enabled = true;
        }

        yield return new WaitForSeconds(interval * 0.2f);

        for (int i = 0; i < Mathf.Min(bulletLaser.Length, muzzleWeapon.Length); i++)
        {
            if (bulletLaser[i] != null)
            {
                bulletLaser[i].enabled = false;
                bulletSounds[i].enabled = false;
            }
        }
    }

    private void RangeReset()
    {
        SetAttackCooldown();
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
            StopCoroutine(GatlingFire());
            StopCoroutine(GatlingAttack());
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

    #region GroundHit

    [Header("Ground Hit Attack")]
    public bool groundHit = false;
    [SerializeField] GameObject groundSmashObj;
    [SerializeField] GameObject groundHitCollider;
    [SerializeField] float distanceFromTargetHit;
    [SerializeField] float groundHitDashSpeed;
    [SerializeField] float groundHitRotSpeed;
    [SerializeField] float groundHitDashDuration;

    public void GroundHitStart()
    {
        Vector3 targetHit = player.position;
        groundHit = true;
        stayPosition = true;
        distanceFromTargetHit = Vector3.Distance(transform.position, targetHit);
        StartCoroutine(GroundAttack(targetHit));
    }

    IEnumerator GroundAttack(Vector3 targetHitPost)
    {
        if (!groundHit) yield break;

        anim.SetBool("GroundHit", false);
        while (distanceFromTargetHit >= 1f)
        {
            distanceFromTargetHit = Vector3.Distance(transform.position, targetHitPost);
            navAgent.SetDestination(targetHitPost);
            navAgent.speed = groundHitDashSpeed;
            yield return null;
        }
        navAgent.SetDestination(transform.position);
        anim.SetBool("GroundHit", true);
    }

    public void GroundHitSpawn()
    {
        StartCoroutine(GroundHitStop());
        Instantiate(groundSmashObj, transform.position, Quaternion.identity);
        StopCoroutine(nameof(GroundAttack));
    }

    IEnumerator GroundHitStop()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("GroundHit", false);
        anim.SetBool("Attacking", false);
        stayPosition = false;
        navAgent.speed = navDefaultSpeed;
        ResetAttack();
        SetAttackCooldown();
        groundHit = false;

        if (!groundHit) yield break;
    }

    public void GroundHitColliderEnable()
    {
        groundHitCollider.SetActive(true);
    }

    public void GroundHitColliderDisable()
    {
        groundHitCollider.SetActive(false);
    }
    #endregion

    #region MissileLaunch-------------------------------------------------------

    [Header("MissileAttack")]
    [SerializeField] GameObject bossMissileObj;
    public bool missileAttack = false;
    [SerializeField] float missileInterval;
    [SerializeField] float missileDuration;

    public void StartMissileAttack()
    {
        missileAttack = true;
        StartCoroutine(MissileAttacking());
    }

    IEnumerator MissileAttacking()
    {
        float time = 0f;
        if (!missileAttack) yield break;

        Invoke(nameof(ResetMissile), missileDuration);
        while (time < missileDuration && missileAttack)
        {
            stayPosition = true;
            navAgent.SetDestination(transform.position);
            time += Time.deltaTime;
            Instantiate(bossMissileObj, player.transform.position, Quaternion.identity);
            float interval = 0f;
            while (interval < missileInterval)
            {
                interval += Time.deltaTime;
                yield return null;
            }
        }
    }

    void ResetMissile()
    {
        anim.SetBool("Attacking", false);
        stayPosition = false;
        missileAttack = false;
        StopCoroutine(MissileAttacking());
    }
    #endregion

    #region GroundSlash---------------------------------------------------------------

    [Header("GroundSlash")]
    [SerializeField] Transform[] objSpawnPost;
    [SerializeField] GameObject groundSlashObj;
    public bool groundSlash = false;
    [SerializeField] float groundSlashDuration;

    public void SpawningGroundSlash()
    {
        stayPosition = true;
        groundSlash = true;
        navAgent.SetDestination(transform.position);
        Quaternion.LookRotation(player.position);
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

    public IEnumerator ResetGroundSlash()
    {
        yield return new WaitForSeconds(groundSlashDuration);
        SetAttackCooldown();
        ResetAttack();
        anim.SetBool("Attacking", false);
        stayPosition = false;
        groundSlash = false;

        if (!groundSlash) yield break;
    }

    #endregion

    #region SweepingAttack--------------------------------------------------------------

    [Header("SweepingLaser")]
    [SerializeField] GameObject[] sweepingLaser;
    public bool sweepingAttack = false;

    public void SweepingLaserEnable()
    {
        sweepingAttack = true;
        foreach(var laser in sweepingLaser)
        {
            laser.SetActive(true);
        }
    }

    public void SweepingLaserDisable()
    {
        anim.SetBool("Attacking", false);
        sweepingAttack = false;
        foreach(var laser in sweepingLaser)
        {
            laser.SetActive(false);
        }
    }
    #endregion

    #region RammingAttack-------------------------------------------------------------
    [Header("RammingAttack")]
    [SerializeField] GameObject rammingCollider;
    [SerializeField] float rammingDuration;
    public bool rammingAttack = false;
    [SerializeField] float rammingDashSpeed;

    public void RammingEnable()
    {
        rammingAttack = true;
        rammingCollider.SetActive(true);
        StartCoroutine(RammingToPlayer());
    }

    public void RammingDisable()
    {
        rammingCollider.SetActive(false);
    }

    IEnumerator RammingToPlayer()
    {
        navAgent.speed = rammingDashSpeed;
        float time = 0f;
        stayPosition = true;

        while (time < rammingDuration)
        {
            time += Time.deltaTime;
            Vector3 forwardOffset = transform.forward * 20f;
            navAgent.SetDestination(player.position + forwardOffset);
            yield return null;
        }
        RammingReset();

        if (!rammingAttack) yield break;
    }

    void RammingReset()
    {
        SetAttackCooldown();
        navAgent.speed = navDefaultSpeed;
        anim.SetBool("Attacking", false);
        RammingDisable();
        stayPosition = false;
        rammingAttack = false;
    }

    #endregion

    #region UltimateAttack-----------------------------------------------------------------------------
    [Header("UltimateAttack")]
    [SerializeField] GameObject LaserObj;
    public bool ultimateAttack = false;
    [SerializeField] float ultimateRotSpeed;
    [SerializeField] float ultimateDuration;

    //anim state
    public void StartUltimateAnim()
    {
        //StartCoroutine(UltimateAttack());
        StartCoroutine(UltimateFacePlayer());
    }

    IEnumerator UltimateFacePlayer()
    {
        if (!ultimateAttack) yield break;

        float time = 0;
        while (time < ultimateDuration)
        {
            time += Time.deltaTime;

            Vector3 direction = player.position - transform.position;
            //direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ultimateRotSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    public void UltimateEnable()
    {
        ultimateAttack = true;
        LaserObj.SetActive(true);
        stayPosition = true;
    }

    public void UltimateDisable()
    {
        LaserObj.SetActive(false);
        //StopCoroutine(UltimateAttack());
        StopCoroutine(UltimateFacePlayer());
        StartCoroutine(UltimateReset());
    }

    //anim state
    IEnumerator UltimateReset()
    {
        yield return new WaitForSeconds(0.5f);
        ultimateAttack = false;
        anim.SetBool("Attacking", false);
        stayPosition = false;
        ResetAttack();
        SetAttackCooldown();

        if (!ultimateAttack) yield return null;
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
