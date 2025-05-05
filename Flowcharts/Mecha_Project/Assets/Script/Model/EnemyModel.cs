using System.Collections;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public EnemyType enemyType;

    public string enemyName;

    [Header("Health")]
    public int maxHealth;
    public int health;
    public int minHealth;

    [Header("Attack")]
    public float sightRange;
    public int attackPower;
    public int attackSpeed;
    public float attackRange;
    public float attackCooldown;

    [Header("Status")]
    public bool isStunt;
    public bool isAttacking;
    public bool isDeath;
    public bool isPatrolling;
    public bool isBlocking;
    public bool isGrounded;
    public bool isHit;
    public bool wasHit = false;
}

public enum EnemyType
    {
        EnemyShort,  // Melee enemies
        EnemyRange,  // Ranged enemies
        MiniBoss,    //MiniBoss
        Boss,        //BossStage
    }
