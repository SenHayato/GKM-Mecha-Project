using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] string sceneToLoad;

    //flag
    //bool isActive = true;
    private void Start()
    {
        loadingScreen.SetActive(false);
    }
    IEnumerator LoadingToScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneToLoad);
        loadingScreen.SetActive(true);

        while(!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
            Debug.Log(progressValue);
            yield return null;
        }
    }

    public void LoadingMonitorButton()
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

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            LoadingMonitorButton();
        }
    }

}
