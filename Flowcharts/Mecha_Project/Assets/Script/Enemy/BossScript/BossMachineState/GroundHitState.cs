using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundHitState : StateMachineBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] BossActive bossActive;
    [SerializeField] GameObject groundSmashEffect;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("StartAttack");

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
        bossActive.groundSmashCollider.SetActive(false);
        navAgent.speed = bossActive.navDefaultSpeed;
        Instantiate(groundSmashEffect, animator.transform.position, Quaternion.Euler(90f, 0, 0f));
    }
}
