using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public TypePower Type;
    //public SkripEnemy Enemies;
    public MechaPlayer Player;
    public PowerUpModel powerUpModel;

    [Header("PowerUP Value")]
    [SerializeField] int AtkPowerUp; 
    [SerializeField] int DefPowerUp;
    [SerializeField] int EngRegenUp;
    [SerializeField] int HPRegenUp;
    [SerializeField] int UltRegenUp;

    [Header("PowerUp Status")]
    public bool isAtkUp;
    public bool isDefUp;
    public float EffectDuration;

    //checker
    int defaultAtk;
    int defaultDef;

    private void Awake()
    {
        powerUpModel = FindFirstObjectByType<PowerUpModel>();
        Player = FindAnyObjectByType<MechaPlayer>();
    }
    private void Start()
    {
        defaultAtk = Player.AttackPow;
        defaultDef = Player.Defence;
    }
    public void Update()
    {
        AtkPowerUp = powerUpModel.AtkPowerUp;
        DefPowerUp = powerUpModel.DefPowerUp;
        EngRegenUp = powerUpModel.EngRegenUp;
        HPRegenUp = powerUpModel.HPRegenUp;
        UltRegenUp = powerUpModel.UltRegenUp;
    }

    public void AtkUp()
    {
        if (isAtkUp)
        {
            Player.AttackPow += AtkPowerUp;
        }
        else
        {
            //Player.AttackPow -= AtkPowerUp;
            Player.AttackPow = defaultAtk;
        }
    }
     public void DefUp()
     {
        if (isDefUp)
        {
            Player.Defence += DefPowerUp;
        }
        else
        {
            //Player.Defence -= DefPowerUp;
            Player.Defence = defaultDef;
        }
     }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (Type)
            {
                case TypePower.AttackUp:
                    isAtkUp = true;
                    AtkUp();
                    break;
                case TypePower.DefenceUp:
                    isDefUp = true;
                    DefUp();
                    break;
                case TypePower.UltimateRegen:
                    Player.Ultimate += UltRegenUp;
                    break;
                case TypePower.EnergyRegen:
                    Player.Energy += EngRegenUp;
                    break;
                case TypePower.HeatlhRegen:
                    Player.Health += HPRegenUp;
                    break;
            }
            gameObject.SetActive(false);
            Destroy(gameObject, EffectDuration);
        }
    }

    private void OnDestroy()
    {
        switch (Type)
        {
            case TypePower.AttackUp:
                 isAtkUp = false;
                 AtkUp();
                 break;
            case TypePower.DefenceUp:
                 isDefUp = false;
                 DefUp();
                 break;
            case TypePower.UltimateRegen:
                 break;
            case TypePower.EnergyRegen:
                 break;
            case TypePower.HeatlhRegen:
                 break;
        }
    }
}



public enum TypePower
{
    HeatlhRegen, AttackUp, DefenceUp, UltimateRegen, EnergyRegen
}
