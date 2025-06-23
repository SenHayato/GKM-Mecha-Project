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
    [SerializeField] string thisSceneName;
    [SerializeField] SceneAsset menuScene;
    [SerializeField] LoadingScript loadingScript;
    private void Awake()
    {
        gameMaster = FindFirstObjectByType<GameMaster>();
        thisSceneName = SceneManager.GetActiveScene().name;
        loadingScript = FindObjectOfType<LoadingScript>();
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
        SceneManager.LoadSceneAsync(thisSceneName);
    }

    public void ExitToMenu()
    {
        gameMaster.gameFinish = true;
        StartCoroutine(loadingScript.LoadingToScene(menuScene.name));
    }
}
