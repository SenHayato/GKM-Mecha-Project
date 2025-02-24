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

    [Header("Status")]
    public bool isAttacking;
    public bool isDeath;
    public bool isMoving;
    public bool isBlocking;
}

public enum EnemyType
{
    EnemyShort, EnemyRange
}
