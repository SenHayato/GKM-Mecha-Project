using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] bool buttonActive;
    [SerializeField] Animation mainMenuAnim;
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] Button[] menuButton;

    //flag
    bool animPlay;

    private void Awake()
    {
        menuMaster = FindFirstObjectByType<MenuMaster>();
        mainMenuAnim = GetComponent<Animation>();
    }

    private void Start()
    {
        menuButton = GetComponentsInChildren<Button>();
        animPlay = false;
    }

    void ButtonMonitoring()
    {
        if (menuMaster.tittleScreenActive)
        {
            menuMaster.mainmenuScreenActive = false;
            buttonActive = false;
        }
        else
        {
            menuMaster.mainmenuScreenActive = true;
            buttonActive = true;
        }
    }

    void ButtonToggle()
    {
        if (buttonActive && menuMaster.mainmenuScreenActive)
        {
            foreach (var button in menuButton)
            {
                button.interactable = true;
            }
        }
        else
        {
            foreach (var button in menuButton)
            {
                button.interactable = false;
            }
        }
    }

    void AnimationManager()
    {
        if (buttonActive && !animPlay)
        {
            mainMenuAnim.Play("MainMenuIn");
            animPlay = true;
        }
    }

    void NewGameButton()
    {
        
    }

    void SettingButton()
    {
        menuMaster.settingScreenActive = true;
    }

    void GalleryButton()
    {
       menuMaster.galleryScreenActive = true;
    }

    void CreditButton()
    {
        menuMaster.creditScreenActive = true;
    }

    void BackButton()
    {
        
    }

    void ExitGameButton()
    {
        Application.Quit();
    }

    void ButtonFunction()
    {
        if (buttonActive)
        {
            NewGameButton();
            SettingButton();
            GalleryButton();
            CreditButton();
            ExitGameButton();
        }

        BackButton();
    }

    private void Update()
    {
        AnimationManager();
        ButtonMonitoring();
        ButtonToggle();
        ButtonFunction();
    }
}
