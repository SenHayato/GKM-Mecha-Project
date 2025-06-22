using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuMaster : MonoBehaviour
{
    public PlayerInput playerInput;

    public GameObject tittleScreen, mainmenuScreen, newGameScreen, settingScreen, creditScene, cgGallery;

    [Header("Menu Script")]
    [SerializeField] MainMenuScript mainMenuScript;
    public SceneAsset firstStage;

    [Header("Canvas Condition")]
    public bool tittleScreenActive;
    public bool mainmenuScreenActive;
    public bool newGameScreenActive; //Tips Screen
    public bool settingScreenActive;
    public bool galleryScreenActive;
    public bool creditScreenActive;
    public void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void ScreenMonitor()
    {
        //tittleScreen
        if (tittleScreenActive)
        {
            tittleScreen.SetActive(true);
        }
        else
        {
            tittleScreen.SetActive(false);
        }

        //newGameScreen
        if (newGameScreenActive)
        {
            newGameScreen.SetActive(true);
        }
        else
        {
            newGameScreen.SetActive(false);
        }

        //mainMenuScreen
        if (mainmenuScreenActive)
        {
            mainmenuScreen.SetActive(true);
        }
        else
        {
            mainmenuScreen.SetActive(false);
        }

        //optionScreen
        if (settingScreenActive)
        {
            settingScreen.SetActive(true);
        }
        else
        {
            settingScreen.SetActive(false);
        }

        //galleryScreen
        if (galleryScreenActive)
        {
            cgGallery.SetActive(true);
        }
        else
        {
            cgGallery.SetActive(false);
        }

        //creditScreen
        if (creditScreenActive)
        {
            creditScene.SetActive(true);
        }
        else
        {
            creditScene.SetActive(false);
        }
    }

    public void BackButton() //Tombol kembali
    {
        mainmenuScreenActive = true;
        if (!mainMenuScript.menuAudio.enabled)
        {
            mainMenuScript.menuAudio.enabled = true;
        }

        //tutup screen lain
        creditScreenActive = false;
        settingScreenActive = false;
        galleryScreenActive = false;
        newGameScreenActive = false;
    }


    private void Update()
    {
        ScreenMonitor();
    }
}
