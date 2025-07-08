using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] GameMaster gameMaster;
    [SerializeField] string thisSceneName;
    [SerializeField] string menuScene;
    [SerializeField] LoadingScript loadingScript;

    [Header("Image Layout")]
    [SerializeField] UnityEngine.UI.Image layoutContainer;
    [SerializeField] Sprite controllerImage;
    [SerializeField] Sprite keyboardImage;

    private bool keyboardLayout = true;
    private void Awake()
    {
        gameMaster = FindObjectOfType<GameMaster>();
        thisSceneName = SceneManager.GetActiveScene().name;
        loadingScript = FindObjectOfType<LoadingScript>();
    }

    public void ChangeLayoutImage()
    {
        if (!keyboardLayout)
        {
            keyboardLayout = true;
            layoutContainer.sprite = keyboardImage;
        }
        else
        {
            keyboardLayout = false;
            layoutContainer.sprite = controllerImage;
        }
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
            yield return null;
        }
    }

    void ExecuteMenu()
    {
        StartCoroutine(LoadingToMenu(menuScene));
    }

    public void ExitToMenu()
    {
        gameMaster.isPaused = false;
        Invoke(nameof(ExecuteMenu), 0.2f);
    }

}
