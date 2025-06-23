using System.Collections;
using UnityEditor;
using UnityEditor.SearchService;
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
    [SerializeField] int enemyInArea = 0;
    [SerializeField] SceneAsset nextScene;

    [Header("GameAdmin")]
    public GameObject PauseMenu;
    public PlayerInput playerInput;
    public GameObject Player;
    public HUDGameManager HUDManager;
    public bool isPaused = false;
    public bool gameFinish = false;
    public PlayerInput input;

    [Header("GameFinish Condition")]
    public bool gameWin = false;
    public bool gameLose = false;
    public SceneAsset loseScreen;

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
    [SerializeField] LoadingScript loadingScript;

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
        loadingScript = FindObjectOfType<LoadingScript>();
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
                QuestText = "Tutorial : Escape from enemy base!";
                countdown = false;
                break;
            case StageType.Stage1:
                QuestText = "Go through the city, find a way out!";
                countdown = false;
                break;
            case StageType.Stage2:
                QuestText = "The enemy is attacking from all directions, HOLD ON!";
                countdown = true;
                if (countdown)
                {
                    Timer();
                }
                break;
            case StageType.StageBoss:
                bossObject = GameObject.FindGameObjectWithTag("Boss");
                bossModel = bossObject.GetComponent<EnemyModel>();
                QuestText = "ELITE-TYPE ENEMY INCOMING, DESTROY IT";
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

            if (nextScene != null && gameWin)
            {
                loadingScript.LoadScene(nextScene.name);
            }

            if (gameLose)
            {
                LoadNextStage(loseScreen.name);
            }
        }
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
        if (isPaused || gameFinish)
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

    void EnemyCounter()
    {
        enemyInArea = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void Update()
    {
        EnemyCounter();
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
        else
        {
            playerInput.enabled = true;
        }
        StageMonitor();
    }

}

public enum StageType
{
    StageTutorial, Stage1, Stage2, StageBoss
}
