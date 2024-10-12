using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMech : MonoBehaviour
{
    [Header("Health")]
    public int Health;
    public int MaxHealth;
    public int MinHealth;

    [Header("Attack")]
    public int AttackPow;
    public float AttackSpd;

    [Header("Ultimate Bar")]
    public int Ultimate;
    public int MaxUltimate;
    public int MinUltimate;

    [Header("Defence")]
    public int Defence;
    public bool Guard;
}
