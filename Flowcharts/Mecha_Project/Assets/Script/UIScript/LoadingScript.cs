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
    //bool isActive = true;
    private void Start()
    {
        loadingScreen.SetActive(false);
        if (pressEnterText != null)
        {
            pressEnterText.SetActive(true);
        }
    }
    IEnumerator LoadingToScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad.name);
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
        StartCoroutine(LoadingToScene());
    }

    //void LoadingMonitor()
    //{
    //    if (isActive)
    //    {
    //        isActive = false;
    //        StartCoroutine(LoadingToScene());
    //    }
    //}

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
