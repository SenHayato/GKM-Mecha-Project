using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] SceneAsset MainMenuScene;
    [SerializeField] UnityEngine.UI.Slider progressBar;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] KeyCode[] skipButtons;
    [SerializeField] Animation anim;
    [SerializeField] float timeBeforeAnim; //sesuaikan dengan cg win screen

    //flag
    bool isPlaying = false;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    private void Start()
    {
        loadingScreen.SetActive(false);
        Invoke(nameof(PlayAnimation), timeBeforeAnim);
    }

    IEnumerator LoadingToScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(MainMenuScene.name);
        loadingScreen.SetActive(true);

        while(!loading.isDone) //kondisi loading belum selesai
        {
            float progressValue = Mathf.Clamp01(loading.progress / 0.9f);
            progressBar.value = progressValue;
            yield return null;
        }
    }

    void PlayAnimation()
    {
        foreach (var button in skipButtons)
        {
            if (Input.GetKeyDown(button))
            {
               return;
            }
        }

        if (!isPlaying)
        {
            isPlaying = true;
            anim.Play("WinScreen");
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

    private bool wasPlaying = false;
    void SkipCG()
    {
        foreach (var button in skipButtons)
        {
            if (Input.GetKeyDown(button))
            {
                wasPlaying = true;
                if (!isPlaying && wasPlaying)
                {
                    wasPlaying = false;
                    isPlaying = true;
                    anim.Play("WinScreen");
                }
            }
        }
    }

    private void Update()
    {
        SkipCG();
    }
}
