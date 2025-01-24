using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class GameMaster : MonoBehaviour
{
    [Header("GameAdmin")]
    public GameObject PauseMenu;
    public GameObject Player;
    public bool isPaused;
    public PlayerInput input;
    //public GameObject Transition;
    public string WinScreen;
    public string LoseScreen;
    public string MainMenu;

    InputAction pauseAction;

    [Header("PlayerAchive")]
    public int KillCount;
    public float timer;
    public bool countdown;
    Vector3 screenCenter;
    //acceptAction, navigateAction, backAction, startAction;

    [Header ("PlayerData")]
    public MechaPlayer MechaData;
    public int PlayerHealth;
    public string LastScene;

    private void Start()
    {
        isPaused = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        input = Player.GetComponent<PlayerInput>();
        pauseAction = input.actions.FindAction("Pause");
        MechaData = Player.GetComponent<MechaPlayer>();
        PlayerHealth = MechaData.Health;

        /*acceptAction = input.actions.FindAction("Accept");
        navigateAction = input.actions.FindAction("Back");
        backAction = input.actions.FindAction("Navigate");
        startAction = input.actions.FindAction("Start");*/

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //Cursor.SetCursor(null, screenCenter, CursorMode.ForceSoftware);
    }
    public void SaveManager()
    {
        SaveSystem.SavePlayer(MechaData);
        Debug.Log("SaveData");
    }

    public void LoadManager()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        PlayerHealth = data.health;
        LastScene = data.sceneName;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = Player.transform.position;
        Debug.Log("LoadData");
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
        PauseButton();
        //HideCursor();
    }
}
