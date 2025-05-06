using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    [SerializeField] string sceneName;

    private void Awake()
    {
        gameMaster = FindFirstObjectByType<GameMaster>();
        sceneName = SceneManager.GetActiveScene().name;
    }

    public void ContinueButton()
    {
        if (gameMaster.isPaused)
        {
            gameMaster.isPaused = false;
        }
        else
        {
            gameMaster.isPaused = true;
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
