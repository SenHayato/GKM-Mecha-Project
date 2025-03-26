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
    public int attackPower;
    public int attackSpeed;
    public float attackRange = 2f;

    [Header("Status")]
    public bool isAttacking;
    public bool isDeath;
    public bool isMoving;
    public bool isBlocking;

    [Header("Debug Visualization")]
    public bool showLineOfSight = true;
    public Color lineOfSightColor = Color.red;
    public bool showFieldOfView = true;
    public Color fieldOfViewColor = new(1f, 1f, 0f, 0.2f);

    [Header("AI Behavior")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;
    public Vector3 startPosition;
    public Vector3 currentDestination;
    public float destinationChangeTimer = 0f;

    [Header("FieldOfView")]
    public float detectionRange = 10f;
    public float viewAngle = 90f;
    public bool useFieldOfView = true;
    public float peripheralViewAngle = 45f;
    public float closeRangeAwareness = 2f;

    [Header("Combat")]
    public float attackCooldown = 2f;
    public float attackTimer = 0f;
    public int attackDamageMin;
    public int attackDamageMax;
    public Transform weaponFirePoint;
}

public enum EnemyType
    {
        EnemyShort,  // Melee enemies
        EnemyRange,  // Ranged enemies
        MiniBoss,    //MiniBoss
        Boss,        //BossStage
    }
