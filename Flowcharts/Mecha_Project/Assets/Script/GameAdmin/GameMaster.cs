using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

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
    public bool isPaused;
    public PlayerInput input;
    //public GameObject Transition;
    public string WinScreen;
    public string LoseScreen;
    public string MainMenu;

    [Header("Transition")]
    public GameObject fadeIn;
    public GameObject fadeOut;

    InputAction pauseAction;

    [Header("PlayerAchive")]
    public int KillCount;
    public float timer;
    public float defaultTimer;
    public string timeFormat;
    public bool countdown;
    Vector3 screenCenter;
    //acceptAction, navigateAction, backAction, startAction;

    [Header ("PlayerData")]
    public MechaPlayer MechaData;
    public int PlayerHealth;
    public string LastScene;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        input = FindAnyObjectByType<PlayerInput>();
        pauseAction = input.actions.FindAction("Pause");
        MechaData = Player.GetComponent<MechaPlayer>();
        HUDManager = FindAnyObjectByType<HUDGameManager>();   
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        fadeIn.SetActive(true);
        fadeOut.SetActive(false);
        isPaused = false;
        PlayerHealth = MechaData.Health;
        //countdown = false; //Hanya perlu di stage gurun Endurance
        timer = defaultTimer;

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
        if (MechaData.isDeath)
        {
            yield return new WaitForSeconds(1f);
            fadeOut.SetActive(true);
        }
    }
    public void LosingScreen()
    {
        SceneManager.LoadScene(LoseScreen);
    }

    public void WinningScreen()
    {
        SceneManager.LoadScene(WinScreen);
    }

    public void BacktoMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }

    public void LoadNextStage(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
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

    public void PauseButton()
    {
        if (pauseAction.triggered)
        {
            Debug.Log("PauseButton");
            if (!isPaused)
            {
                isPaused = true;
            } else
            {
                isPaused = false;
            }
            Paused();
        }
    }

    public void Update()
    {
        //PauseButton();
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
    }
}

public enum StageType
{
    StageTutorial, Stage1, Stage2, StageBoss
}
