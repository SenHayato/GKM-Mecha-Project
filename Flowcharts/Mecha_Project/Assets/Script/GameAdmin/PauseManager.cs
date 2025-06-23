using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameMaster gameMaster;
    [SerializeField] string thisSceneName;
    [SerializeField] SceneAsset menuScene;
    [SerializeField] LoadingScript loadingScript;
    private void Awake()
    {
        gameMaster = FindObjectOfType<GameMaster>();
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

    public IEnumerator LoadingToMenu(string SceneName)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(SceneName);
        loadingScript.loadingScreen.SetActive(true);

        while (!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            loadingScript.progressBar.value = progressValue;
            Debug.Log(progressValue);
            yield return null;
        }
    }

    void ExecuteMenu()
    {
        StartCoroutine(LoadingToMenu(menuScene.name));
    }

    public void ExitToMenu()
    {
        gameMaster.isPaused = false;
        Invoke(nameof(ExecuteMenu), 0.2f);
    }

}
