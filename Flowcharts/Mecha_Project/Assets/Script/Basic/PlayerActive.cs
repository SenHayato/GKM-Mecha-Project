using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActive : MonoBehaviour
{
    PlayerInput gameInput;

    // Input
    public InputAction moveAction, jumpAction, flyUp, shootAction, scopeAction, skill1Action, skill2Action,
        flyDown, blockAction, dashAction, selectButton, ultimateAction;

    public MechaPlayer Mecha;
    public GameMaster GameMaster;
    public CameraActive CameraAct;
    public Transform cameraPivot;
    public GameObject skillHitBox;

    [Header("Player Status")]
    public float speed;
    public float jumpForce;
    public float gravity;
    public bool isGrounded;
    public float verticalVelocity;
    public float fallMultiplier;
    public float rotationSpeed;
    private bool wasAiming;

    public CharacterController controller;
    public Animator anim;

    // Start is called before the first frame update
    public void Start()
    {
        GameMaster = FindAnyObjectByType<GameMaster>();
        CameraAct = FindAnyObjectByType<CameraActive>();
        rotationSpeed = CameraAct.rotationSpeed;
        cameraPivot = CameraAct.cameraPivot;
        skillHitBox = GameObject.Find("SkillHitBox");
        skillHitBox.SetActive(false);

        Mecha = GetComponent<MechaPlayer>();
        gameInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        moveAction = gameInput.actions.FindAction("Movement");
        jumpAction = gameInput.actions.FindAction("Jump");
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

        wasAiming = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Hukum Fisika COY
        ApplyGravity();
        SelectButtonPress();

        //Class untuk player
        PlayerJump();
        DashPlayer();
        BlockPlayer();
        UltimatePlayer();
        ScopeMode();
        Shooting();
        Hovering();
        Death();
        RelativeMovement();
        StartCoroutine(Skill());
    }

    public void DashPlayer()
    {
       if (dashAction.IsPressed() && !Mecha.isDashing)
       {
            Mecha.isDashing = true;
            speed *= 3f;
            Debug.Log("Dash");
       }

       if (!dashAction.IsPressed() && Mecha.isDashing)
       {
            Mecha.isDashing = false;
            anim.SetFloat("Move", 0f);
            speed /= 3f;
       }
    }

    public void RelativeMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        if (!Mecha.isDeath)
        {
            if (moveInput != Vector2.zero)
            {
                // Arah relatif dari kamera
                Vector3 forward = cameraPivot.transform.forward;
                Vector3 right = cameraPivot.transform.right;

                // Memberikan nilai 0 pada nilai Y agar tetap Horizontal
                forward.y = 0f;
                right.y = 0f;

                // Normalisasi vektor
                forward.Normalize();
                right.Normalize();

                // Hitung arah pergerakan relatif terhadap kamera
                Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

                if (!Mecha.isAiming)
                {
                    // Rotasi player menghadap arah pergerakan
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                }
                // Gerakkan player relatif terhadap kamera
                controller.Move(speed * Time.deltaTime * moveDirection);

                // Atur animasi pergerakan player
                anim.SetFloat("Move", 1f);
                anim.SetBool("IsMove", true);

                if (Mecha.isDashing)
                {
                    anim.SetFloat("Move", 2f);
                }
            }
            else
            {
                anim.SetFloat("Move", 0f);
                anim.SetBool("IsMove", false);
            }
        }
    }

    public void Hovering()
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
    public void PlayerJump()
    {
        if (jumpAction.triggered && isGrounded)
        {
            Debug.Log("Jump");
            verticalVelocity = jumpForce;
            isGrounded = false;
            if (anim.GetBool("IsJump") != true)
            {
                anim.SetBool("IsJump", true);
            }
        }
        if (isGrounded)
        {
            anim.SetBool("IsJump", false);
        }
    }

    public void ApplyGravity()
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

    public void BlockPlayer()
    {
        if (blockAction.triggered)
        {
            Debug.Log("Block");
        }
    }

    public void UltimatePlayer()
    {
        if (ultimateAction.triggered)
        {
            Debug.Log("Ultimate");
        }
    }

    public void SelectButtonPress()
    {
        if (selectButton.triggered)
        {
            Debug.Log("SelectButton");
        }
    }

    public void ScopeMode()
    {
        if (scopeAction.IsPressed())
        {
            Mecha.isAiming = true;
        }
        else
        {
            Mecha.isAiming = false;
        }
         //ScopeCondition
        if (Mecha.isAiming)
        {
            Debug.Log("Scope On");
            anim.SetBool("IsAiming", true);
            CameraAct.ScopeCamera();
        }
        else
        {
            anim.SetBool("IsAiming", false);
            CameraAct.ScopeCamera();
        }

        if (Mecha.isAiming != wasAiming)
        {
            if (Mecha.isAiming)
            {
                CameraAct.rotationSpeed /= 4f;
                speed /= 2f;
            }
            else
            {
                CameraAct.rotationSpeed *= 4f;
                speed *= 2f;
            }
        }
        wasAiming = Mecha.isAiming;
    }

    public void Shooting()
    {
        if (shootAction.IsPressed())
        {
            Mecha.isShooting = true;
        }
        else
        {
            Mecha.isShooting = false;
        }

        if (Mecha.isShooting)
        {
            if (Mecha.isAiming)
            {
                CameraAct.ScopeCamera();
            }
            else
            {
                CameraAct.ShootingCamera();
            }
            Debug.Log("Shoot ON");
            anim.SetBool("IsShooting", true);
        }
        else
        {
            anim.SetBool("IsShooting", false);
        }
        
    }

    public IEnumerator Skill()
    {
        if (skill1Action.triggered)
        {
            Debug.Log("Skill 1 Aktif Korotine");
            Mecha.isSkill1 = true;
            skillHitBox.SetActive(true);
            anim.SetTrigger("IsSkill1");
            yield return new WaitForSeconds(2.20f);
            Mecha.isSkill1 = false;
            skillHitBox.SetActive(false);
        }

        if (skill2Action.triggered)
        {
            Debug.Log("Skill 1 Aktif Korotine");
            Mecha.isSkill1 = true;
            skillHitBox.SetActive(true);
            anim.SetTrigger("IsSkill1");
            yield return new WaitForSeconds(2.20f);
            Mecha.isSkill1 = false;
            skillHitBox.SetActive(false);
        }
    }

    public void Death()
    {
        if (Mecha.Health <= Mecha.MinHealth)
        {
            Mecha.Health = Mecha.MinHealth;
            Mecha.isDeath = true;

            if (Mecha.isDeath)
            {
                speed = 0f;
                anim.SetBool("IsDeath", true);
                Invoke(nameof(ToLoseCG), 5f);
            }
            else
            {
                anim.SetBool("IsDeath", false);
            }
        }
    }

    public void ToLoseCG()
    {
        GameMaster.LosingScreen();
    }
}