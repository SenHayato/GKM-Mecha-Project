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
                enemyModel.attackCooldown = 5f;
            }
        }
    }

    private void CheckPlayer()
    {
        playerInMelee = Physics.CheckSphere(transform.position, meleeRadius, playerLayer);
        playerInRange = Physics.CheckSphere(transform.position, shootRange, playerLayer);
    }

    #region GroundSlash

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

    #region SweepingAttack

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

    #region RammingAttack
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

    #region UltimateAttack
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
