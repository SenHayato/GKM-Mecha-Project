using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] SceneAsset sceneToLoad;
    [SerializeField] bool isTipsScreen;
    [SerializeField] GameObject pressEnterText; //opsional jika ada button konfirmasi

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
    IEnumerator LoadingToScene(string SceneName)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(SceneName);
        loadingScreen.SetActive(true);

        while(!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
            Debug.Log(progressValue);
            yield return null;
        }
    }

    public void LoadingMonitorButton() //Bisa buat tipsScreen
    {
        StartCoroutine(LoadingToScene(sceneToLoad.name));
    }

    public void LoadScene(string NextScene)
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(LoadingToScene(NextScene));
        }
    }

    private void Update()
    {
        //LoadingMonitor();

        if (Input.GetKeyDown(KeyCode.Keypad7)) //nanti ganti ke key return saat build dan ditambah bool
        {
            if (pressEnterText != null)
            {
                pressEnterText.SetActive(false);
            }
            LoadingMonitorButton();
        }
    }

}
