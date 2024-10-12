using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool Guard;

    public void Death()
    {
        SceneManager.LoadScene("");
    }
}
