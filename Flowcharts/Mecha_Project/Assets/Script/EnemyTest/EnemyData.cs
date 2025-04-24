using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
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
}

//public enum EnemyType //masih ada enemytype di enemy model yg lama
//{
//    EnemyShort,  // Melee enemies
//    EnemyRange,  // Ranged enemies
//    MiniBoss,    //MiniBoss
//    Boss,        //BossStage
//}
