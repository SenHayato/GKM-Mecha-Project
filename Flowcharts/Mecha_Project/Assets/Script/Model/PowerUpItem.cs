using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public TypePower Type;
    //public SkripEnemy Enemies;
    public MechaPlayer Player;

    [Header("PowerUP Value")]
    public int AtkPowerUp;
    public int DefPowerUp;
    public int HvrRegenUp;
    public int HPRegenUp;
    public int UltRegenUp;

    [Header("PowerUp Status")]
    public bool isAtkUp;
    public bool isDefUp;
    public float EffectDuration;

    public void Start()
    {
        //Enemies = FindObjectsOfType<SkripEnemy>();
        Player = FindAnyObjectByType<MechaPlayer>();
    }

    public void AtkUp()
    {
        if (isAtkUp)
        {
            Player.AttackPow += AtkPowerUp;
        }
        else
        {
            Player.AttackPow -= AtkPowerUp;
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
            Player.Defence -= DefPowerUp;
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
                    break;
                case TypePower.HoverRegen:
                    break;
                case TypePower.HeatlhRegen:
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
            case TypePower.HoverRegen:
                 break;
            case TypePower.HeatlhRegen:
                 break;
        }
    }
}



public enum TypePower
{
    HeatlhRegen, AttackUp, DefenceUp, UltimateRegen, HoverRegen
}
