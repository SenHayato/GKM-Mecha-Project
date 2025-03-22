using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] GameMaster gameMaster;

    [Header("CutScene Video")]
    [SerializeField] VideoClip introCutScene;
    [SerializeField] VideoClip stage1CutScene;
    [SerializeField] VideoClip stage2CutScene;
    [SerializeField] VideoClip stageBossCutScene;
    [SerializeField] VideoClip outroCutScene;

    [Header("Audio Monitor")]
    [SerializeField] AudioSource[] audioSources;

    [Header("Video Set Up")]
    public GameObject videoPlayerOBJ;
    public VideoPlayer videoPlayer;
    [SerializeField] bool isPlaying = false;

    //flag
    bool trigger = true;
    private void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
        audioSources = FindObjectsOfType<AudioSource>();
    }

    private void Start()
    {
        foreach (var source in audioSources)
        {
            if (!source.CompareTag("MusicManager"))
            {
                if (source != null)
                {
                    source.enabled = false;
                }
            }
        }
        videoPlayerOBJ.SetActive(true);
        //switch (gameMaster.StageType)
        //{
        //    case StageType.StageTutorial:
        //        videoPlayer.clip = introCutScene;
        //        break;
        //    case StageType.Stage1:
        //        videoPlayer.clip = stage1CutScene;
        //        break;
        //    case StageType.Stage2:
        //        videoPlayer.clip = stage2CutScene;
        //        break;
        //    case StageType.StageBoss:
        //        videoPlayer.clip = stageBossCutScene;
        //        break;
        //}
    }

    IEnumerator VideoMonitoring()
    {
        if (trigger && !isPlaying)
        {
            isPlaying = true;
            videoPlayer.Play();
        }
        yield return new WaitForSecondsRealtime((float)videoPlayer.clip.length + 2f);
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.enabled = true;
            }
        }
        videoPlayerOBJ.SetActive(false);
        trigger = false;
        isPlaying = false;

    }

    void GameTimeManager()
    {
        if (isPlaying)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("Jalan");
            Time.timeScale = 1f;
        }
    }

    private void Update()
    {
        GameTimeManager();
        StartCoroutine(VideoMonitoring());
    }
}
