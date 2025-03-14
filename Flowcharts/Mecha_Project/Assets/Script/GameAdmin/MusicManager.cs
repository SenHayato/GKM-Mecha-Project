using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    [SerializeField] MechaPlayer mechaPlayer;

    [Header("Music Library")]
    [SerializeField] AudioClip[] musicLibrary;
    [SerializeField] AudioSource musicSource;

    void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
