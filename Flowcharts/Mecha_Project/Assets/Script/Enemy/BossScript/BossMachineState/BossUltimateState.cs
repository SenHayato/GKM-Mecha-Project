using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossUltimateState : StateMachineBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] BossActive bossActive;
    [SerializeField] EnemyModel enemyModel;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent = animator.GetComponent<NavMeshAgent>();
        bossActive = animator.GetComponent<BossActive>();
        enemyModel = animator.GetComponent<EnemyModel>();

        bossActive.ultimating = true;
        bossActive.ultimateLookAtPlayer = true;
        navAgent.speed = 0;
        bossActive.UltimateResetState();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossActive.ultimateLookAtPlayer = false;
        bossActive.enableLaserUltimate = true;
        navAgent.speed = 0;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.speed = bossActive.navDefaultSpeed;
        bossActive.enableLaserUltimate = false;
        bossActive.ultimating = false;
        enemyModel.isAttacking = false;
    }
}
