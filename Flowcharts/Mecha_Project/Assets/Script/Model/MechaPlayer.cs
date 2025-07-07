using Unity.VisualScripting;
using UnityEngine;

public class MechaPlayer : MonoBehaviour
{
    [Header("Health")]
    public int Health;
    public int MaxHealth;
    public int MinHealth;
    public int crtiticalHealth;

    [Header("Attack")]
    public int AttackPow;
    public bool isAttackUp;
    //public float AttackSpd;

    [Header("Awakening Bar")]
    public float Awakening;
    public float MaxAwakening;
    [HideInInspector] public float MinAwakening = 0;
    public float AwakeningRegen;
    public float AwakeningDuration;
    public bool UsingAwakening;
    public bool awakeningReady;
    public int awakeningAttack;

    [Header("Ultimate Bar")]
    public int Ultimate;
    public int MaxUltimate; //100
    [HideInInspector] public int MinUltimate = 0;
    public int UltRegenValue;
    public int UltDamage;
    public float UltInterval;
    public float UltDuration;
    [HideInInspector] public bool UltimateRegen = false;
    public bool UltimateReady;
    public bool UsingUltimate = false;

    [Header("Energy")]
    public int Energy;
    public int MaxEnergy; //100
    [HideInInspector] public int MinEnergy = 0; //0
    public int EngRegenValue;
    public int EnergyCost; //50
    public bool EnergyRegen;

    [Header("Skill Condition")]
    //skillPlayer
    public bool readySkill1;
    public bool usingSkill1 = false;    
    public float skill1Time;
    public float cooldownSkill1;
    public int skill1Damage;
    public float skill1Duration;

    //skillWeapon (Heavy Skill)
    public bool readySkill2;
    public bool usingSkill2 = false;
    public int skill2MaxBar; //100f
    public int skill2Bar;
    public int skill2Damage;
    public float skill2Duration; //sesuaikan sama animasi skill2

    [Header("Defence")]
    public int Defence;

    [Header("Status")]
    public bool undefeat;
    public bool isJumping;
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

