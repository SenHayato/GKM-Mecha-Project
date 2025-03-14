using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    [SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] AudioSource musicSource;

    [Header("Music Library")]
    [SerializeField] AudioClip tutorialBGM;
    [SerializeField] AudioClip cityStageBGM;
    [SerializeField] AudioClip dessertStageBGM;
    [SerializeField] AudioClip finalStageBGM;

    [Header("Music SetUp")]
    [SerializeField] AudioMixer bgmMixer;

    void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
        mechaPlayer = FindFirstObjectByType<MechaPlayer>();
    }

    void Start()
    {
        
    }

    void AudioMonitor()
    {

    }

    void Update()
    {
        AudioMonitor();
    }
}
