using System.Collections;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public EnemyType enemyType;

    [Header("Health")]
    public int maxHealth;
    public int health;
    public int minHealth;

    [Header("Attack")]
    public int attackPower;
    public int attackSpeed;
    public float attackRange = 2f;
    public float detectionRange = 10f;

    [Header("Status")]
    public bool isAttacking;
    public bool isDeath;
    public bool isMoving;
    public bool isBlocking;

    [Header("Debug Visualization")]
    public bool showLineOfSight = true;
    public Color lineOfSightColor = Color.red;

    [Header("AI Behavior")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    public Vector3 startPosition;
    public Vector3 currentDestination;
    public float destinationChangeTimer = 0f;

    [Header("Combat")]
    public float attackCooldown = 2f;
    public float attackTimer = 0f;
    public Transform weaponFirePoint;
}

public enum EnemyType
    {
        EnemyShort,  // Melee enemies
        EnemyRange,  // Ranged enemies
        MiniBoss,    //MiniBoss
        Boss,        //BossStage
    }
