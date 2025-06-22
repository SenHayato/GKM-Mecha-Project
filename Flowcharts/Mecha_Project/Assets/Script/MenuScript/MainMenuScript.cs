using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] bool buttonActive;
    [SerializeField] Animation mainMenuAnim;
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] Button[] menuButton;
    [SerializeField] LoadingScript loadingScript;
    public AudioSource menuAudio;
    public bool tipsScreenActive = false;

    //flag
    bool animPlay;

    private void Awake()
    {
        menuAudio = GetComponent<AudioSource>();
        menuMaster = FindFirstObjectByType<MenuMaster>();
        mainMenuAnim = GetComponent<Animation>();
        loadingScript = FindObjectOfType<LoadingScript>();
    }

    private void Start()
    {
        menuButton = GetComponentsInChildren<Button>();
        animPlay = false;
        menuAudio.enabled = true;
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

    public void NewGameButton()
    {
        menuMaster.newGameScreenActive = true;
        tipsScreenActive = true;
        menuAudio.volume = Mathf.Lerp(1f, 0.2f, Time.deltaTime / 0.005f);
    }

    public void SettingButton()
    {
        menuMaster.settingScreenActive = true;
        Debug.Log("Setting Test");
    }

    public void GalleryButton()
    {
        menuMaster.galleryScreenActive = true;
        menuAudio.enabled = false;
    }

    public void CreditButton()
    {
        menuMaster.creditScreenActive = true;
        menuAudio.enabled = false;
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    void TipsScreen()
    {
        if (Input.GetKeyDown(loadingScript.pressToSkip) && tipsScreenActive)
        {
            loadingScript.LoadScene(menuMaster.firstStage.name);
        }
    }
    private void Update()
    {
        TipsScreen();
        AnimationManager();
        ButtonMonitoring();
        ButtonToggle();
    }
}
