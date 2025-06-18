using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] string MainMenuScene;
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }

    IEnumerator LoadingToScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(MainMenuScene);
        loadingScreen.SetActive(true);

        while(!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
            Debug.Log(progressValue);
            yield return null;
        }
    }

    void LoadingMonitorButton()
    {
        StartCoroutine(LoadingToScene());
    }


    public void BackToScene()
    {
        Invoke(nameof(LoadingMonitorButton), 0.2f);
    }
}
