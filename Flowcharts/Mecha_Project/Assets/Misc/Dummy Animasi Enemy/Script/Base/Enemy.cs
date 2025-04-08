using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable
{
    [field: SerializeField] public float MaxHealth { get; set; } = 100f;
    public float CurrentHealth { get; set; }
    public CharacterController controller { get; set; }
    public bool IsFacingPlayer { get; set; } = true;

    public Transform playerTransform;

    #region State Machine Variables

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }


    #endregion

    public float RandomMovementRange = 5f;
    public float RandomMovementSpeed = 1f;

    #region IdleVariables
    #endregion
    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;

        controller = GetComponent<CharacterController>();

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region Health/Die Function
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth < 0 )
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Movement Function
    public void MoveEnemy(Vector3 normalizedDirection)
    {
        // Membuat enemy jalan
       controller.Move(normalizedDirection * Time.deltaTime);

        //rotate enemy ke player
        if (playerTransform != null)
        {
            CheckForFacingPlayer(playerTransform.position);
        }
    }

    public void CheckForFacingPlayer(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        directionToPlayer.y = 0f; // Untuk Menghindari Enemy melihat kebawah ataupun keatas

        if (directionToPlayer == Vector3.zero) return;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
    }

    public void CheckForFacingDirection(Vector3 velocity)
    {
        if (velocity == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
    }
    #endregion

    #region Animation Triggers

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }
    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound
    }

    #endregion
}
