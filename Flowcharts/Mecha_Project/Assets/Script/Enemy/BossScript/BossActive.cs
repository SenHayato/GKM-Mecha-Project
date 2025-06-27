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
    int SecondStageHealth;

    [Header("AttackState")]
    public bool playerInMelee;
    public bool playerInRange;
    public float preparingTime;
    public bool hasAttacked;

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
    public Transform muzzleGatRight;
    public LineRenderer bulletGatRight; 
    public Transform muzzleGatLeft;
    public LineRenderer bulletGatLeft;

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
        //LockRotation();
        SecondStage();
        CheckPlayer();

        if (enemyModel.isGrounded)
        {
            AttackCooldown();
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
            navDefaultSpeed = 12f;
            chaseSpeed = 16f;
            //material berubah merah
        }
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

      //void LockRotation()
    //{
    //    Quaternion lockRotate = transform.rotation;
    //    lockRotate.x = 0;
    //    lockRotate.z = 0;
    //    transform.rotation = lockRotate;
    //}

}
