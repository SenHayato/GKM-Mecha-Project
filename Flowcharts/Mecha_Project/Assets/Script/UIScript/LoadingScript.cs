using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public UnityEngine.UI.Slider progressBar;
    public GameObject loadingScreen;
    [SerializeField] string sceneToLoad;
    [SerializeField] GameObject pressEnterText; //opsional jika ada button konfirmasi
    public KeyCode pressToSkip;

    //flag
    public bool isActive = false;
    private void Start()
    {
        loadingScreen.SetActive(false);
        if (pressEnterText != null)
        {
            pressEnterText.SetActive(true);
        }
    }
    public IEnumerator LoadingToScene(string SceneName)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(SceneName);
        loadingScreen.SetActive(true);

        while (!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
            yield return null;
        }
    }

    public void LoadingMonitorButton() //Bisa buat tipsScreen
    {
        if (sceneToLoad != null)
        {
            StartCoroutine(LoadingToScene(sceneToLoad));
        }
    }

    public void LoadScene(string NextScene)
    {
        if (!isActive)
        {
            isActive = true;
            if (pressEnterText != null)
            {
                pressEnterText.SetActive(false);
            }
            StartCoroutine(LoadingToScene(NextScene));
        }
    }
}
