using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShort : MonoBehaviour
{
   [Header("Heath")]
    public int Health;
    public int MinHealth;
    public int MaxHealth;

    [Header("Attack")]
    public int AttackPow;
    public int AttackSpd;
}
