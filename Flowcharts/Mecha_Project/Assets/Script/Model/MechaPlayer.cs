using UnityEngine;

public class MechaPlayer : MonoBehaviour
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

    [Header("Hover Bar")]
    public int Hover;
    public int MaxHover;
    public int MinHover;

    [Header("Defence")]
    public int Defence;

    [Header("Status")]
    public bool isDeath;
    public bool isGuard;
    public bool isHovering;
    public bool isAiming;
    public bool isShooting;
    public bool isDashing;

    [Header("Skill Condition")]
    public bool isSkill1;
    public bool isSkill2;

    [Header("Player Position")]
    public Transform PlayerPosition;
}

