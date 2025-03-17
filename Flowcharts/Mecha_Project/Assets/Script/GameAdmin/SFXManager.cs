using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] MechaPlayer mechaPlayer;

    [Header("SFX Library")]
    [SerializeField] AudioClip ultimateUse;
    [SerializeField] AudioClip warningAlarm;
    [SerializeField] AudioClip ultimateFull;
    [SerializeField] AudioClip energyFull;
    [SerializeField] AudioClip playerExplode;
    [SerializeField] AudioClip sprintSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] bool isActive = false;

    private void Awake()
    {
        mechaPlayer = GetComponentInParent<MechaPlayer>();
    }

    void PlayerMonitor()
    {

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
