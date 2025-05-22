using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundSlashState : StateMachineBehaviour
{
    [SerializeField] BossActive bossActive;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] EnemyModel enemyModel;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossActive = animator.GetComponent<BossActive>();
        navAgent = animator.GetComponent<NavMeshAgent>();
        enemyModel = animator.GetComponent<EnemyModel>();

        bossActive.GroundSlashResetState();
        bossActive.spawnSlash = true;
        navAgent.speed = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.speed = 0;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.speed = bossActive.navDefaultSpeed;
        enemyModel.isAttacking = false;
    }
}
