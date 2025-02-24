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
    public int MaxUltimate; //100
    public int MinUltimate; //0
    public int UltRegenValue;
    public bool UltimateRegen;
    public int UltDamage;

    [Header("Energy")]
    public int Energy;
    public int MaxEnergy; //100
    public int MinEnergy; //0
    public int EngRegenValue;
    public int EnergyCost; //50
    public bool EnergyRegen;

    [Header("Skill Condition")]
    //skill1
    public bool readySkill1;
    public float skill1Time;
    public float cooldownSkill1;
    public int skill1Damage;
    //skill2
    public bool readySkill2;
    public float skill2Time;
    public float cooldownSkill2;
    public int skill2Damage;

    [Header("Defence")]
    public int Defence;

    [Header("Status")]
    public bool isDeath;
    public bool isBlocking;
    public bool isHovering;
    public bool isAiming;
    public bool isShooting;
    public bool isDashing;
    public bool isReloading;
    public bool isBoosting;

    [Header("Player Position")]
    public Transform PlayerPosition;
}

