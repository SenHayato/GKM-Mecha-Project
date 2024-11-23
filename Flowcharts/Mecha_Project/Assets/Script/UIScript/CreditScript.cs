using UnityEngine;
using UnityEngine.InputSystem;

public class CreditScript : MonoBehaviour
{
    public GameObject mainmenuScreen, creditScreen;
    PlayerInput UImanager;
    InputAction skip;

    public void Start()
    {
        UImanager = GetComponent<PlayerInput>();
        skip = UImanager.actions.FindAction("Accept");
    }

    public void SkipCreditButton()
    {
         mainmenuScreen.SetActive(true);
         creditScreen.SetActive(false);
    }

    public void Update()
    {
        SkipCredit();
    }

    public void SkipCredit()
    {
        if (skip.triggered)
        {
            mainmenuScreen.SetActive(true);
            creditScreen.SetActive(false);
        }
    }
}
