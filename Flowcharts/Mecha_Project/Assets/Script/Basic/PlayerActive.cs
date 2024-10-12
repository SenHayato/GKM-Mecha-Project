using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActive : MonoBehaviour
{
    PlayerInput gameInput;

    // Input
    InputAction moveAction, jumpAction, startButton, flyUp, shootAction, scopeAction, skill1Action, skill2Action,
        flyDown, blockAction, dashAction, selectButton, ultimateAction;

    public GameObject PauseMenu;
    public bool isPaused;
    public MechaPlayer Player;

    [Header("Player Status")]
    public float speed;
    public float jumpForce;
    public float gravity;
    public bool isGrounded;
    public float verticalVelocity;
    public float fallMultiplier;

    private CharacterController controller;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        gameInput = GetComponent<PlayerInput>();
        moveAction = gameInput.actions.FindAction("Movement");
        jumpAction = gameInput.actions.FindAction("Jump");
        startButton = gameInput.actions.FindAction("Pause");
        blockAction = gameInput.actions.FindAction("Block");
        dashAction = gameInput.actions.FindAction("Dash");
        selectButton = gameInput.actions.FindAction("Select");
        flyUp = gameInput.actions.FindAction("FlyUp");
        flyDown = gameInput.actions.FindAction("FlyDown");
        ultimateAction = gameInput.actions.FindAction("Ultimate");
        skill1Action = gameInput.actions.FindAction("Skill 1");
        skill2Action = gameInput.actions.FindAction("Skill 2");
        shootAction = gameInput.actions.FindAction("Shoot");
        scopeAction = gameInput.actions.FindAction("Scope");

        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Hukum Fisika COY
        ApplyGravity();
        OpenPause();
        SelectButtonPress();

        //Class untuk player
        MovePlayer();
        PlayerJump();
        DashPlayer();
        BlockPlayer();
        UltimatePlayer();
        ScopeMode();
        Shooting();
        Skill();
        Hovering();
        Death();
    }

    void MovePlayer()
    {
        if (moveAction.IsPressed())
        {
            //Debug.Log(moveAction.ReadValue<Vector2>());
            Vector2 direction = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(direction.x, 0, direction.y) * speed;
            controller.Move(move * Time.deltaTime);
            anim.SetFloat("Move", 1f);
            anim.SetBool("IsMove", true);
        }
        else
        {
            anim.SetFloat("Move", 0f);
            anim.SetBool("IsMove", false);
        }
    }

    void Hovering()
    {
        if (flyUp.IsPressed())
        {
            Debug.Log("FlyUp");
        }

        if (flyDown.IsPressed())
        {
            Debug.Log("FlyDown");
        }
    }
    void PlayerJump()
    {
        if (jumpAction.triggered && isGrounded)
        {
            Debug.Log("Jump");
            verticalVelocity = jumpForce;
            isGrounded = false;
            anim.SetBool("IsJump", true);
        }
        else
        {
            anim.SetBool("IsJump", false);

        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * fallMultiplier * Time.deltaTime;
        } else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 gravityMovement = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        controller.Move(gravityMovement);

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
            isGrounded = true;
        }
    }

    void OpenPause()
    {
        if (startButton.triggered)
        {
            if (isPaused == false)
            {
                PauseMenu.SetActive(true);
                isPaused = true;
                Time.timeScale = 0f;
            } else if (isPaused == true)
            {
                isPaused = false;
                PauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }

    void DashPlayer()
    {
        if (dashAction.triggered)
        {
            Debug.Log("Dash");
        }
    }

    void BlockPlayer()
    {
        if (blockAction.triggered)
        {
            Debug.Log("Block");
        }
    }

    void UltimatePlayer()
    {
        if (ultimateAction.triggered)
        {
            Debug.Log("Ultimate");
        }
    }

    void SelectButtonPress()
    {
        if (selectButton.triggered)
        {
            Debug.Log("SelectButton");
        }
    }

    void ScopeMode()
    {
        if (scopeAction.IsPressed())
        {
            Debug.Log("Scope ON");
        }
    }

    void Shooting()
    {
        if (shootAction.IsPressed())
        {
            Debug.Log("Shoot ON");
        }
    }

    void Skill()
    {
        if (skill1Action.triggered)
        {
            Debug.Log("Skill 1 Activated");
        }

        if (skill2Action.triggered)
        {
            Debug.Log("Skill 2 Activated");
        }
    }

    void Death()
    {
        
    }
}