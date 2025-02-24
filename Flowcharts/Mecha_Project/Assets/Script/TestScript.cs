using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    PlayerInput gameInput;

    InputAction spawnActions;
    public GameObject enemy;
    private void Awake()
    {
        gameInput = FindAnyObjectByType<PlayerInput>();
    }

    private void Start()
    {
        spawnActions = gameInput.actions.FindAction("TestSpawn");
    }

    public void Spawnner()
    {
        if (spawnActions.triggered)
        {
            Instantiate(enemy);
        }
    }

    private void Update()
    {
        Spawnner();
    }
}
