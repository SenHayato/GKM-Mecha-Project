using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _direction;
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        _targetPos = GetRandomPointInCircle();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(enemy.IsChased)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }

        _direction = (_targetPos - enemy.transform.position).normalized;

        enemy.MoveEnemy(_direction * enemy.RandomMovementSpeed);

        if ((enemy.transform.position - _targetPos).sqrMagnitude < 0.01f)
        {
            _targetPos = GetRandomPointInCircle();
        }

        if (_direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(_direction, Vector3.up);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, toRotation, 5f * Time.deltaTime);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private Vector3 GetRandomPointInCircle()
    {
        Vector3 randomOffset = Random.insideUnitSphere * enemy.RandomMovementRange;
        randomOffset.y = 0f; // We only want movement in the XZ plane
        return enemy.transform.position + randomOffset;
    }
}
