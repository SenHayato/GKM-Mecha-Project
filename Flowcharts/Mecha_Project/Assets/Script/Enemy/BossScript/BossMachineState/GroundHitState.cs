using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundHitState : StateMachineBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] BossActive bossActive;
    [SerializeField] GameObject groundSmashEffect;
    [SerializeField] EnemyModel enemyModel;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("StartAttack");

        enemyModel = animator.GetComponent<EnemyModel>();
        navAgent = animator.GetComponent<NavMeshAgent>();
        navAgent.speed = 0f;

        bossActive = animator.GetComponent<BossActive>();
        bossActive.groundSmashCollider.SetActive(true);
        bossActive.hasTeleported = true;
        bossActive.GroundHitStateReset();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossActive.hasTeleported = false;
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 targetGround = animator.transform.position;
        targetGround.y = 0.25f;
        Instantiate(groundSmashEffect, targetGround, Quaternion.identity);
        bossActive.groundSmashCollider.SetActive(false);
        navAgent.speed = bossActive.navDefaultSpeed;
        enemyModel.isAttacking = false;
    }
}
