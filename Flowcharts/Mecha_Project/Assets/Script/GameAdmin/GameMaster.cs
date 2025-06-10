using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    [Header("QuestInfo")]
    public StageType StageType;
    public string QuestText;
    public int checkPointReach;

    [Header("GameAdmin")]
    public GameObject PauseMenu;
    public PlayerInput playerInput;
    public GameObject Player;
    public HUDGameManager HUDManager;
    public bool isPaused = false;
    public bool gameFinish = false;
    public PlayerInput input;
    public string NextScene; //untuk last boss next scene diisi Win Scene

    [Header("GameFinish Condition")]
    public bool gameWin = false;
    public bool gameLose = false;
    public string LoseScreen;
    public string MainMenu;

    [Header("Transition")]
    public GameObject fadeIn;
    public GameObject fadeOut;

    InputAction pauseAction;

    [Header("PlayerAchive")]
    public int KillCount;
    public float timer;
    public float defaultTimer; //saat game berjalan waktu default 3 menit
    public string timeFormat;
    public bool countdown;
    Vector3 screenCenter;
    //acceptAction, navigateAction, backAction, startAction;

    [Header ("PlayerData")]
    public MechaPlayer MechaData;
    public int PlayerHealth;
    public string LastScene;

    [Header("Reference")]
    [SerializeField] CutSceneManager cutSceneManager;

    [Header("SkyAsset")]
    [SerializeField] Material skyMaterial;
    [SerializeField] float skyRotSpeed;
    private float skyCurrentRotation;

    //flag
    GameObject bossObject;
    EnemyModel bossModel;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        input = FindAnyObjectByType<PlayerInput>();
        pauseAction = input.actions.FindAction("Pause");
        MechaData = Player.GetComponent<MechaPlayer>();
        HUDManager = FindAnyObjectByType<HUDGameManager>();   
        playerInput = GetComponent<PlayerInput>();
        cutSceneManager = FindFirstObjectByType<CutSceneManager>();
    }

    private void Start()
    {
        Time.timeScale = 1f;
        fadeIn.SetActive(true);
        fadeOut.SetActive(false);
        PlayerHealth = MechaData.Health;
        timer = defaultTimer;
        HUDManager.questUIAnim.Play("QuestInfoIn");

        switch (StageType)
        {
            case StageType.StageTutorial:
                QuestText = "Ini stage tutorial";
                countdown = false;
                break;
            case StageType.Stage1:
                QuestText = "Ini stage 1";
                countdown = false;
                break;
            case StageType.Stage2:
                QuestText = "Ini stage 2";
                countdown = true;
                if (countdown)
                {
                    Timer();
                }
                break;
            case StageType.StageBoss:
                bossObject = GameObject.FindGameObjectWithTag("Boss");
                bossModel = bossObject.GetComponent<EnemyModel>();
                QuestText = "Ini stage Boss";
                countdown = false;
                break;
        }

        /*acceptAction = input.actions.FindAction("Accept");
        navigateAction = input.actions.FindAction("Back");
        backAction = input.actions.FindAction("Navigate");
        startAction = input.actions.FindAction("Start");*/

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //Cursor.SetCursor(null, screenCenter, CursorMode.ForceSoftware);
    }

    //public void SaveManager()
    //{
    //    SaveSystem.SavePlayer(MechaData);
    //    Debug.Log("SaveData");
    //}

    //public void LoadManager()
    //{
    //    PlayerData data = SaveSystem.LoadPlayer();
    //    PlayerHealth = data.health;
    //    LastScene = data.sceneName;

    //    Vector3 position;
    //    position.x = data.position[0];
    //    position.y = data.position[1];
    //    position.z = data.position[2];
    //    transform.position = Player.transform.position;
    //    Debug.Log("LoadData");
    //}

    IEnumerator TransitionManager()
    {
        if (gameFinish)
        {
            if (!isPaused)
            {
                Time.timeScale = 0.5f;
            }
            fadeOut.SetActive(true);
            yield return new WaitForSeconds(7f);
            if (NextScene != null && gameWin)
            {
                LoadNextStage(NextScene);
            }

            if (gameLose)
            {
                LoadNextStage(LoseScreen);
            }
        }
    }
    public void LosingScreen()
    {
        SceneManager.LoadScene(LoseScreen);
    }

    public void BacktoMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }

    public void LoadNextStage(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    void StageMonitor()
    {
        switch (StageType)
        {
            case StageType.Stage2:
                if (timer <= 0 && !MechaData.isDeath)
                {
                    gameFinish = true;
                    gameWin = true;
                }
                break;
            case StageType.StageBoss:
                if (bossModel.health <= 0)
                {
                    gameFinish = true;
                    gameWin = true;
                }
                break;
        }
    }

    public void Timer()
    {
        timer -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timeFormat = string.Format("{00:00}:{1:00}", minutes, seconds);
        HUDManager.timerText.text = timeFormat;

        if (timer <= 20f)
        {
            HUDManager.timerText.color = Color.red;
        }
        else
        {
            HUDManager.timerText.color = Color.white;
        }
    }

    public void Paused()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            Debug.Log("Sudah di pause");
            PauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            PauseMenu.SetActive(false);
        }
    }

    public void HideCursor()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Cursor.SetCursor(null, screenCenter, CursorMode.ForceSoftware);
        }
    }

    public void BlockInput()
    {
        if (cutSceneManager.isPlaying)
        {
            playerInput.enabled = false;
        }
        else
        {
            playerInput.enabled = true;
        }
    }

    public void PauseButton()
    {
        if (!cutSceneManager.isPlaying)
        {
            if (pauseAction.triggered)
            {
                Debug.Log("PauseButton");
                if (!isPaused)
                {
                    isPaused = true;
                }
                else
                {
                    isPaused = false;
                }
            }
            Paused();
        }
    }

    public void RotateSky()
    {
        if (skyMaterial != null)
        {
            skyCurrentRotation += skyRotSpeed * Time.deltaTime;
            skyMaterial.SetFloat("_Rotation", skyCurrentRotation);
        }
    }

    public void Update()
    {
        BlockInput();
        PauseButton();
        HideCursor();
        if (countdown)
        {
            Timer();
        }
        StartCoroutine(TransitionManager());
        if (MechaData.isDeath)
        {
            playerInput.enabled = false;
        }
        StageMonitor();
        RotateSky();
    }

}

public enum StageType
{
    StageTutorial, Stage1, Stage2, StageBoss
}
