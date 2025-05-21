using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RammingAttackState : StateMachineBehaviour
{
    [SerializeField] BossActive bossActive;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] EnemyModel enemyModel;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyModel = animator.GetComponent<EnemyModel>();
        navAgent = animator.GetComponent<NavMeshAgent>();
        bossActive = animator.GetComponent<BossActive>();
        navAgent.speed = 0;
        bossActive.rammingLookAtPlayer = true;
        bossActive.RammingAttackResetState();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossActive.rammingLookAtPlayer = false;
        bossActive.rammingAttacking = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.ResetTrigger("StartAttack");
        bossActive.rammingAttacking = false;
        navAgent.speed = bossActive.navDefaultSpeed;
        enemyModel.isAttacking = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
