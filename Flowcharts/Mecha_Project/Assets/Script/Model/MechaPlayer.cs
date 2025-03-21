using Unity.VisualScripting;
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

    [Header("Awakening Bar")]
    public float Awakening;
    public float MaxAwakening;
    [HideInInspector] public float MinAwakening = 0;
    public float AwakeningRegen;
    public bool isAwakening;
    public bool awakeningReady;

    [Header("Ultimate Bar")]
    public int Ultimate;
    public int MaxUltimate; //100
    [HideInInspector] public int MinUltimate = 0;
    public int UltRegenValue;
    public int UltDamage;
    public float UltInterval;
    public float UltDuration;
    public bool UltimateRegen;
    public bool UltimateReady;
    public bool UsingUltimate;

    [Header("Energy")]
    public int Energy;
    public int MaxEnergy; //100
    [HideInInspector] public int MinEnergy = 0; //0
    public int EngRegenValue;
    public int EnergyCost; //50
    public bool EnergyRegen;

    [Header("Skill Condition")]
    //skill1
    public bool readySkill1;
    public bool usingSkill1;
    public float skill1Time;
    public float cooldownSkill1;
    public int skill1Damage;
    public float skill1Duration;
    //skill2
    public bool readySkill2;
    public bool usingSkill2;
    public float skill2Time;
    public float cooldownSkill2;
    public int skill2Damage;
    public float skill2Duration;

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
    public bool isIdle;
    public float defaultSpeed;

    [Header("Player Position")]
    public Transform PlayerPosition;
}

