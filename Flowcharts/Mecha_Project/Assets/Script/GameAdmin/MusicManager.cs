using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    //[SerializeField] MechaPlayer mechaPlayer;
    [SerializeField] AudioSource jingleSource;
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

    [Header("Game Jingle")]
    //[SerializeField] AudioClip gameStart;
    [SerializeField] AudioClip gameEnd1;
    [SerializeField] AudioClip gameEnd2;
    [SerializeField] AudioClip gameEnd3;

    [Header("Music SetUp")]
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] bool isPlaying;

    [Header("Awakening Music Setting")]
    [SerializeField] AudioSource awakeningSource;
    bool awakeningActive = false;


    //check
    bool wasPlaying = false;
    bool musicTransition = false;
    GameObject bossObj;
    EnemyModel bossModel;
    [SerializeField] float timelerp = 0f;
    float musicLerp = 0f;
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
            awakeningSource.volume = Mathf.Lerp(1f, 0f, timelerp);
            if (!musicTransition)
            {
                timelerp += Time.deltaTime / 2f;
                musicSource.volume = Mathf.Lerp(0f, 1f, timelerp);
            }
        }
    }
    //void MusicWhenPause()
    //{
    //    //if (gameMaster.isPaused)
    //    //{
    //    //    musicSource.volume = 0.5f;
    //    //}
    //    //else
    //    //{
    //    //    musicSource.volume = 1f;
    //    //}
    //}

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
                if (gameMaster.timer <= 63f && gameMaster.timer >= 60f)
                {
                    Debug.Log("Asu");
                    musicTransition = true;
                    if (musicSource.volume > 0f)
                    {
                        musicLerp += Time.deltaTime / 2f;
                        musicSource.volume = Mathf.Lerp(1f, 0f, musicLerp);
                    }
                }
                else
                {
                    musicTransition = false;
                }

                AudioClip newClip = gameMaster.timer <= 60f ? dessertStageBGMAlter : dessertStageBGM;

                if (musicSource.clip != newClip)
                {
                    musicSource.clip = newClip;
                    musicSource.Play();
                    if (!mechaPlayer.UsingAwakening)
                    {
                        musicSource.volume = 1f;
                    }
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

    void GameFinish()
    {
        switch (gameMaster.StageType)
        {
            case StageType.StageTutorial:
                jingleSource.clip = gameEnd1;
                break;
            case StageType.Stage1:
                jingleSource.clip = gameEnd1;
                break;
            case StageType.Stage2:
                jingleSource.clip = gameEnd2;
                break;
            case StageType.StageBoss:
                jingleSource.clip = gameEnd3;
                break;
        }
        if (gameMaster.gameFinish && gameMaster.gameWin)
        {
           
            jingleSource.enabled = true;
            musicSource.enabled = false;
            Debug.Log("Jinggle Musik");
        }
        else
        {
            jingleSource.enabled = false;
            musicSource.enabled = true;
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
        GameFinish();
        Invoke(nameof(AudioMonitor), (float)cutSceneManager.videoPlayer.clip.length - 4f);
        AwakeningMusicFlap();
        //MusicWhenPause();
        //AudioMonitor();
        //MusicPlay();
    }
}
