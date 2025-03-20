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
    [SerializeField] CutSceneManager cutSceneManager;

    [Header("Music Library")]
    [SerializeField] AudioClip tutorialBGM;
    [SerializeField] AudioClip cityStageBGM;
    [SerializeField] AudioClip dessertStageBGM;
    [SerializeField] AudioClip dessertStageBGMAlter;
    [SerializeField] AudioClip finalStageBGM;
    [SerializeField] AudioClip finalStageBGMAlter;

    [Header("Music SetUp")]
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] bool isPlaying;


    //check
    bool wasPlaying = false;
    GameObject bossObj;
    EnemyModel bossModel;
    void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
        //mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        musicSource = GetComponentInChildren<AudioSource>();
        cutSceneManager = FindFirstObjectByType<CutSceneManager>();
    }

    private void Start()
    {
        switch (gameMaster.StageType) //hanya untuk boss stage
        {
            case StageType.StageBoss:
                bossObj = GameObject.FindGameObjectWithTag("Boss");
                bossModel = bossObj.GetComponent<EnemyModel>();
                break;
        }
    }

    void AudioMonitor()
    {
        switch (gameMaster.StageType)
        {
            case StageType.StageTutorial:
                musicSource.clip = tutorialBGM;
                break;
            case StageType.Stage1: //City Stage
                musicSource.clip = cityStageBGM;
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
                AudioClip newClip = gameMaster.timer <= 60f ? dessertStageBGMAlter : dessertStageBGM;
                if (musicSource.clip != newClip)
                {
                    musicSource.clip = newClip;
                    musicSource.Play();
                }
                break;
            case StageType.StageBoss: //Dessert Stage Final
                AudioClip bossClip = bossModel.health <= 50000 ? finalStageBGMAlter : finalStageBGM;
                if (musicSource.clip != bossClip)
                {
                    musicSource.clip = bossClip;
                    musicSource.Play();
                }
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
        Invoke(nameof(AudioMonitor), (float)cutSceneManager.videoPlayer.clip.length - 4f);
        //AudioMonitor();
        //MusicPlay();
    }
}
