using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    //[SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] AudioSource musicSource;
    [SerializeField] GameObject musicObj;
    [SerializeField] CutSceneManager cutSceneManager;
    [SerializeField] MechaPlayer mechaPlayer;

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

    [Header("Awakening Music Setting")]
    [SerializeField] AudioSource awakeningSource;
    bool awakeningActive = false;


    //check
    bool wasPlaying = false;
    GameObject bossObj;
    EnemyModel bossModel;
    [SerializeField] float timelerp = 0;
    void Awake()
    {
        gameMaster = GetComponent<GameMaster>();
        //mechaPlayer = FindFirstObjectByType<MechaPlayer>();
        musicSource = GetComponentInChildren<AudioSource>();
        cutSceneManager = FindFirstObjectByType<CutSceneManager>();
        mechaPlayer = FindAnyObjectByType<MechaPlayer>();
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

    void AwakeningMusicFlap()
    {
        if (mechaPlayer.UsingAwakening && !awakeningActive)
        {
            awakeningSource.Play();
            awakeningSource.volume = 1f;
            musicSource.volume = 0f;
            timelerp = 0f;
            awakeningActive = true;
        }
        
        if (!mechaPlayer.UsingAwakening)
        {
            awakeningActive = false;
            timelerp += Time.deltaTime / 2f;
            musicSource.volume = Mathf.Lerp(0f, 1f, timelerp);
            awakeningSource.volume = Mathf.Lerp(1f , 0f, timelerp);
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
        AwakeningMusicFlap();
        //AudioMonitor();
        //MusicPlay();
    }
}
