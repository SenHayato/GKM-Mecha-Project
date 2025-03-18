using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] PlayerActive playerActive;

    [Header("SFX Library")]
    [SerializeField] AudioClip ultimateFull;
    [SerializeField] AudioClip energyFull;
    [SerializeField] AudioClip playerExplode;
    public AudioClip waypointReach;
    [SerializeField] bool isActive = false;

    [Header("AudioSetUp")]
    public AudioSource audioSource;

    //flag
    float thrusterLerp = 0f;

    private void Awake()
    {
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        playerActive = FindFirstObjectByType<PlayerActive>();
    }

    void PlayerMonitor()
    {
        if (mechaPlayer.isDashing)
        {
            thrusterLerp += Time.deltaTime / 0.5f;
            playerActive.thrusterSound.volume = Mathf.Lerp(0.5f, 1f, thrusterLerp);
        }
        else
        {
            playerActive.thrusterSound.volume = 0.5f;
            thrusterLerp = 0f;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMonitor();
    }
}
