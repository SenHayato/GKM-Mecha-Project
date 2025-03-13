using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpModel : MonoBehaviour
{
    [Header("PowerUP Value")]
    public int AtkPowerUp;
    public int DefPowerUp;
    public int EngRegenUp;
    public int HPRegenUp;
    public int UltRegenUp;

    [Header("Animation")]
    public float spinSpeed;
    public Vector3 rotationAxis;
}
