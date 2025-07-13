using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TittleScreen : MonoBehaviour
{
    //PlayerInput playerInput;
    [SerializeField] MenuMaster menuMaster;
    [SerializeField] bool isPressed;
    [SerializeField] Animation tittleScreenAnim;
    [SerializeField] GameObject tittleButton;
    [SerializeField] AudioSource tittleClickSFX;
    [SerializeField] Animation animGtk;

    private void Awake()
    {
        tittleClickSFX = GetComponent<AudioSource>();
        gameObject.SetActive(true);
        //playerInput = FindFirstObjectByType<PlayerInput>();
        menuMaster = FindFirstObjectByType<MenuMaster>();
        tittleScreenAnim = GetComponent<Animation>();
    }

    private void Start()
    {
        menuMaster.tittleScreenActive = true;
        isPressed = false;
        tittleButton.SetActive(true);
    }

    IEnumerator PressButtonEnter()
    {
        if (!isPressed && (Keyboard.current.anyKey.isPressed || Gamepad.current != null && Gamepad.current.buttonEast.isPressed))
        {
            isPressed = true;
            tittleClickSFX.Play();
            tittleButton.SetActive(false);
            tittleScreenAnim.Play("TittleScreenClose");
            animGtk.Play("MenuGTK");
            yield return new WaitForSeconds(tittleScreenAnim.clip.length);
            menuMaster.tittleScreenActive = false;
            menuMaster.mainmenuScreenActive = true;
            gameObject.SetActive(false); //tittle screen
        }
    }

    private void Update()
    {
        StartCoroutine(PressButtonEnter());
    }
}
