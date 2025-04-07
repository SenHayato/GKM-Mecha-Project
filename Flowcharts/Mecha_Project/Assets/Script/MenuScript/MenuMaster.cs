using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuMaster : MonoBehaviour
{
    public PlayerInput playerInput;

    public GameObject tittleScreen, mainmenuScreen, settingScreen, creditScene, cgGallery;

    [Header("Canvas Condition")]
    public bool tittleScreenActive;
    public bool mainmenuScreenActive;
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

    private void Update()
    {
        ScreenMonitor();
    }
}
