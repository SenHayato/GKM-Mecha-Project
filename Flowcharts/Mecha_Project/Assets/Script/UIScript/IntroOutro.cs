using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroOutro : MonoBehaviour
{
    PlayerInput UImanager;
    InputAction skip;
    public string namenextScene;

    public void Start()
    {
        UImanager = GetComponent<PlayerInput>();
        skip = UImanager.actions.FindAction("Accept");
    }

    public void Update()
    {
        SkipCG();
    }

    void SkipCG()
    {
        if (skip.triggered)
        {
            Invoke(nameof(ToNextScene), 3f);
        }
    }

    public void ToNextScene()
    {
        SceneManager.LoadScene(namenextScene);
    }
}
