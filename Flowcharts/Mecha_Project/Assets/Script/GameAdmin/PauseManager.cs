using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    [SerializeField] string sceneName;
    [SerializeField] SceneAsset menuScene;
    [SerializeField] LoadingScript loadingScript;

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
        SceneManager.LoadSceneAsync(menuScene.name);
    }
}
