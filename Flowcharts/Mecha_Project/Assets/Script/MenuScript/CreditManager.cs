using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditManager : MonoBehaviour
{
    [Header("Reference SetUp")]
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] MainMenuScript mainMenuScript;
    //[SerializeField] bool isPressed;

    [Header("Credit Properties")]
    [SerializeField] GameObject creditText;
    [SerializeField] float creditDuration;
    [SerializeField] float scrollSpeed;
    [SerializeField] Transform defaultTextPosition;
    //[SerializeField] AudioSource creditSource;

    //flag
    float timeLerp = 0f;

    void Awake()
    {
        //creditSource = GetComponent<AudioSource>();
        menuMaster = FindFirstObjectByType<MenuMaster>();
        mainMenuScript = FindFirstObjectByType<MainMenuScript>();
    }

    //private void Start()
    //{
    //    //creditDuration = creditSource.clip.length;
    //}

    private void OnEnable()
    {
        //isPressed = false;
        timeLerp = 0f;
        creditText.transform.position = defaultTextPosition.position;
        StartCoroutine(CreditPlaying());
    }

    IEnumerator CreditPlaying()
    {
        while (timeLerp < 1f) // Selama belum mencapai durasi
        {
            timeLerp += Time.deltaTime / creditDuration;
            float y = Mathf.Lerp(
                defaultTextPosition.position.y,
                defaultTextPosition.position.y + scrollSpeed,
                timeLerp
            );

            creditText.transform.position = new Vector2(defaultTextPosition.position.x, y);
            yield return null;
        }

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
