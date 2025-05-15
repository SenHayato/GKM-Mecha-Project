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
    [SerializeField] GameObject attackObj1;
    [SerializeField] GameObject attackObj2;
    [SerializeField] GameObject attackObj3;
    [SerializeField] GameObject attackObj4;
    [SerializeField] GameObject ultimateObj;

    [Header("AttackToggler")]
    [SerializeField] bool attacking1;
    [SerializeField] bool attacking2;
    [SerializeField] bool attacking3;
    [SerializeField] bool attacking4;
    [SerializeField] bool attacking5;
    [SerializeField] bool attacking6;
    [SerializeField] bool ultimating;

    public override void Attacking()
    {
        Debug.Log("Boss Attack");
        CheckPlayer();
    }

    private void CheckPlayer()
    {
        playerInMelee = Physics.CheckSphere(transform.position, meleeRadius, playerLayer);
        playerInRange = Physics.CheckSphere(transform.position, shootRange, playerLayer);
    }

    private void FireRifle()
    {

    }

    private void FireGatling()
    {

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
