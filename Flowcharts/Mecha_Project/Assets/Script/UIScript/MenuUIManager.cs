using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    PlayerInput UImanager;

    //Input
    InputAction startButton;
    public GameObject mainmenuScreen, optionScreen, creditScene, cgGallery;

    public void Start()
    {
        UImanager = GetComponent<PlayerInput>();
        startButton = UImanager.actions.FindAction("Start");
    }

    public void Update()
    {
        PressStartPAD();
    }

    void PressStartPAD()
    {
        if (startButton.triggered)
        {
            PressStartButton();
        }
    }


    //Individual Button
    public void PressStartButton()
    {
        mainmenuScreen.SetActive(true);
    }

    public void OptionsButton()
    {
        optionScreen.SetActive(true);
        mainmenuScreen.SetActive(false);
    }
    public void CreditScene()
    {
        creditScene.SetActive(true);
        mainmenuScreen.SetActive(false);
    }

    public void NewGameButton()
    {
        Invoke(nameof(NewGameLoad), 1.5f);
    }

    public void CGGalleryScene()
    {
        cgGallery.SetActive(true);
        mainmenuScreen.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //SceneManagerLoad
    public void NewGameLoad()
    {
        SceneManager.LoadScene("TipsScreen");
    }
}
