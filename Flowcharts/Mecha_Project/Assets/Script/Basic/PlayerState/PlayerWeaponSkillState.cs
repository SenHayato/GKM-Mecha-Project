using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponSkillState : StateMachineBehaviour
{
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] PlayerActive playerActive;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mechaPlayer = animator.GetComponent<MechaPlayer>();
        playerActive = animator.GetComponent<PlayerActive>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mechaPlayer.usingSkill2 = true;
        playerActive.skillBusy = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mechaPlayer.readySkill2 = false;
        mechaPlayer.usingSkill2 = false;
        playerActive.skillBusy = false;
        animator.ResetTrigger("IsSkill2");
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
