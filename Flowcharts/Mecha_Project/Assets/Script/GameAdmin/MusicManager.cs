using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    //[SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] AudioSource musicSource;
    [SerializeField] GameObject musicObj;

    [Header("Music Library")]
    //[SerializeField] AudioClip tutorialBGM;
    //[SerializeField] AudioClip cityStageBGM;
    [SerializeField] AudioClip dessertStageBGM;
    [SerializeField] AudioClip dessertStageBGMAlter;
    //[SerializeField] AudioClip finalStageBGM;
    //[SerializeField] AudioClip finalStageBGMAlter;

    [Header("Music SetUp")]
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] bool isPlaying;


    //check
    bool wasPlaying = false;
    void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
        //mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        musicSource = GetComponentInChildren<AudioSource>();
    }

    //private void Start()
    //{
    //    musicObj.SetActive(true);
    //}

    void AudioMonitor()
    {
        switch (gameMaster.StageType)
        {
            case StageType.StageTutorial:
                break;
            case StageType.Stage1: //City Stage
                break;
            case StageType.Stage2: // Dessert Stage
                //AudioClip newClip;
                //if (gameMaster.timer < 60f)
                //{
                //    newClip = dessertStageBGMAlter;
                //}
                //else
                //{
                //    newClip = dessertStageBGM;
                //}
                AudioClip newClip = gameMaster.timer < 60f ? dessertStageBGMAlter : dessertStageBGM;
                if (musicSource.clip != newClip)
                {
                    musicSource.clip = newClip;
                    musicSource.Play();
                }
                break;
            case StageType.StageBoss: //Dessert Stage Final
                break;
        }

        if (isPlaying && !wasPlaying)
        {
            wasPlaying = true;
            musicSource.Play();
        }
    }

    //void MusicPlay()
    //{
    //    if (isPlaying && !wasPlaying)
    //    {
    //        musicSource.PlayOneShot();
    //    }
    //}


    void Update()
    {
        AudioMonitor();
        //MusicPlay();
    }
}
