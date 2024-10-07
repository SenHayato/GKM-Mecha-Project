using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkipCGGallery : MonoBehaviour
{
    public GameObject[] thisObjects;
    public PlayerInput UImanager;
    InputAction skip;
    void Start()
    {
       UImanager = GetComponent<PlayerInput>();
       skip = UImanager.actions.FindAction("Accept");
    }

    void Update()
    {
        SkipCGthis();
    }

    public void SkipCGthis()
    {
        if (skip.triggered)
        {
            foreach (GameObject obj in thisObjects)
            {
                obj.SetActive(false);
            }
        }
    }
}
