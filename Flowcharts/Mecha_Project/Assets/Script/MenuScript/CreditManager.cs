using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreditManager : MonoBehaviour
{
    [Header("Reference SetUp")]
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] bool creditPlaying;
    [SerializeField] bool isPressed;

    [Header("Credit Properties")]
    [SerializeField] GameObject creditText;
    [SerializeField] float creditDuration;
    [SerializeField] float scrollSpeed;
    //[SerializeField] AudioSource creditSource;

    //flag
    float timeLerp = 0f;

    void Awake()
    {
        //creditSource = GetComponent<AudioSource>();
        menuMaster = FindFirstObjectByType<MenuMaster>();
    }

    private void Start()
    {
        //creditDuration = creditSource.clip.length;
        creditPlaying = true;
        isPressed = false;
    }

    IEnumerator CreditPlaying()
    {
        if (creditPlaying)
        {
            timeLerp += Time.deltaTime / creditDuration;
            creditText.transform.position = new Vector2 (transform.position.x, scrollSpeed * timeLerp);
            yield return new WaitForSeconds(creditDuration);
            creditPlaying = false;
            menuMaster.mainmenuScreenActive = true;
            menuMaster.creditScreenActive = false;
        }
    }

    void SkipCreditButton()
    {
        if (!isPressed && (Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
        {
            isPressed = true;
            creditPlaying = false;
            menuMaster.mainmenuScreenActive = true;
            menuMaster.creditScreenActive = false;
        }
    }

    void Update()
    {
        StartCoroutine(CreditPlaying());
        SkipCreditButton();
    }
}
