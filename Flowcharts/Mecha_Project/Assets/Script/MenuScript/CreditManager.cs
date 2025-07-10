using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CreditManager : MonoBehaviour
{
    [Header("Reference SetUp")]
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] MainMenuScript mainMenuScript;
    //[SerializeField] bool isPressed;

    [Header("Credit Properties")]
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] float creditDuration;
    //[SerializeField] AudioSource creditSource;

    //flag
    //float timeLerp = 0f;

    void Awake()
    {
        //creditSource = GetComponent<AudioSource>();
        menuMaster = FindFirstObjectByType<MenuMaster>();
        mainMenuScript = FindFirstObjectByType<MainMenuScript>();
        creditDuration = (float) videoPlayer.length;
    }

    private void OnEnable()
    {
        //isPressed = false;
        //timeLerp = 0f;
        StartCoroutine(CreditPlaying());
    }

    IEnumerator CreditPlaying()
    {
        yield return new WaitForSeconds(creditDuration);

        // Setelah durasi berakhir
        menuMaster.mainmenuScreenActive = true;
        mainMenuScript.menuAudio.enabled = true;
        menuMaster.creditScreenActive = false;
    }
    void SkipCreditButton()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            menuMaster.mainmenuScreenActive = true;
            mainMenuScript.menuAudio.enabled = true;
            menuMaster.creditScreenActive = false;
        }
    }

    void Update()
    {
        SkipCreditButton();
    }
}
