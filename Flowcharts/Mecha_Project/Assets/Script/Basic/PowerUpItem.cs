using System.Collections;
using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public TypePower Type;
    //public SkripEnemy Enemies;
    public MechaPlayer Player;
    public PowerUpModel powerUpModel;
    private float spinSpeed;
    //private Vector3 rotationAxis;
    public CombatVoiceActive voiceActive;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject triggerParticle;
    [SerializeField] Collider powerCollider;
    [SerializeField] GameObject UIMap;
    [SerializeField] GameObject beaconPower;

    [Header("PowerUP Value")]
    private int AtkPowerUp; 
    private int DefPowerUp;
    private int EngRegenUp;
    private int HPRegenUp;
    private int UltRegenUp;

    [Header("PowerUp Status")]
    private bool isAtkUp;
    private bool isDefUp;
    private float EffectDuration;

    //checker
    int defaultAtk;
    int defaultDef;

    private void Awake()
    {
        powerCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        voiceActive = FindAnyObjectByType<CombatVoiceActive>();
        powerUpModel = FindFirstObjectByType<PowerUpModel>();
        Player = FindAnyObjectByType<MechaPlayer>();
    }
    private void Start()
    {
        beaconPower.SetActive(true);
        UIMap.SetActive(true);
        powerCollider.enabled = true;
        meshRenderer.enabled = true;
        triggerParticle.SetActive(false);

        //set value
        defaultAtk = Player.AttackPow;
        defaultDef = Player.Defence;
        EffectDuration = powerUpModel.EffectDuration;
        spinSpeed = powerUpModel.spinSpeed;
        AtkPowerUp = powerUpModel.AtkPowerUp;
        DefPowerUp = powerUpModel.DefPowerUp;
        EngRegenUp = powerUpModel.EngRegenUp;
        HPRegenUp = powerUpModel.HPRegenUp;
        UltRegenUp = powerUpModel.UltRegenUp;
    }

    public void Update()
    {
        Muter();
    }

    void Muter()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + spinSpeed * Time.deltaTime, 0f);
    }

    public void AtkUp()
    {
        if (isAtkUp)
        {
            Player.AttackPow += AtkPowerUp;
        }
        else
        {
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
            Player.Defence = defaultDef;
        }
     }

    private void OnTriggerEnter(Collider other)
    {
        triggerParticle.SetActive(true);
        if (other.CompareTag("Player"))
        {
            beaconPower.SetActive(false);
            voiceActive.PowerUpGet();
            switch (Type)
            {
                case TypePower.AttackUp:
                    Player.isAttackUp = true;
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
            UIMap.SetActive(false);
            powerCollider.enabled = false;
            meshRenderer.enabled = false;
            Destroy(gameObject, EffectDuration);
        }
    }

    private void OnDestroy()
    {
        switch (Type)
        {
            case TypePower.AttackUp:
                isAtkUp = false;
                Player.isAttackUp = false;
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
