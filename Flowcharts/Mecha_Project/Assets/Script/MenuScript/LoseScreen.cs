using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] string mainmenuScene;
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] GameObject loadingScreen;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }

    IEnumerator LoadingToScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(mainmenuScene);
        loadingScreen.SetActive(true);

        while(!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
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
