using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PowerUpModel : MonoBehaviour
{
    public float EffectDuration;

    [Header("PowerUP Value")]
    public int AtkPowerUp;
    public int DefPowerUp;
    public int EngRegenUp;
    public int HPRegenUp;
    public int UltRegenUp;

    [Header("Animation")]
    public float spinSpeed;

    [Header("Beacon")]
    public bool needBeacon;
    //public Vector3 rotationAxis;
}
