using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActive : MonoBehaviour
{
    [SerializeField] PlayerInput gameInput;

    // Input
    public InputAction moveAction, jumpAction, flyUp, shootAction, scopeAction, skill1Action, skill2Action,
        flyDown, blockAction, dashAction, selectButton, ultimateAction, interaction, reloadAction, boostAction, awakeningAction, resetCameraAction;
    [Header("Reference")]
    public MechaPlayer Mecha;
    public GameMaster GameMaster;
    public CameraActive CameraAct;
    public CombatVoiceActive combatVoiceAct;
    public MusicManager musicManager;
    public CutSceneManager cutSceneManager;

    [Header("Player Set Up")]
    public Transform cameraPivot;
    public Transform playerPosition;
    public CameraEffect cameraEffect;
    public WeaponRaycast Weapon;
    public GameObject windEffect;
    public HashSet<string> enemyTags = new() { "Enemy", "Boss", "MiniBoss" };

    [Header("Default Parameter")]
    [SerializeField] float walkRotValue;
    private float defaultSpeed;
    private float dashSpeed;
    private int defaultAttack;
    private int defaultUltDamage;
    private int defaultDefence;

    [Header("WeaponSlot")]
    [SerializeField] Transform WeaponSlot;

    [Header("Skill dan Ultimate")]
    public bool mechaInAwakenState = false;
    public LayerMask enemyLayer;
    public GameObject weaponSkillObj;
    public GameObject skill2HitBox;
    [SerializeField] GameObject playerSkillObj;
    [SerializeField] GameObject playerSkillObjDash;
    [SerializeField] Transform playerSkillSpawn;
    [SerializeField] GameObject ultimateObj;

    [Header("BoostEffect")]
    [SerializeField] private float normalBoostSpeed;
    [SerializeField] private float fastBoostSpeed;
    [SerializeField] private float dashBoostSpeed;

    [Header("Mecha Sound")]
    [SerializeField] AudioSource hitSound;
    public AudioSource thrusterSound;

    [Header("Player Status")]
    public float speed;
    public float jumpForce;
    public float gravity;
    public bool isGrounded;
    public float verticalVelocity;
    public float fallMultiplier;
    public float rotationSpeed;
    private bool wasAiming;
    public bool skillBusy;
    public CharacterController controller;
    public Animator anim;

    [Header("Boost Config")]
    public float boostDuration;
    public float boostSpeedMultiplier;
    [SerializeField] float boostDistance;
    [SerializeField] float boostDirectionLerp = 0;

    [Header("Attribut & VFX")]
    [SerializeField] GameObject shieldObj;
    [SerializeField] GameObject explodedVFX;
    [SerializeField] GameObject slashEffect;
    [SerializeField] GameObject smokeVFX;
    [SerializeField] GameObject jumpDust;
    [SerializeField] GameObject trailDust;
    [SerializeField] ParticleSystem thusterParticle;
    [SerializeField] Material thusterJetVFX; //ambil dari asset file agar terpasang global
    [SerializeField] Material miniThrusterVFX; //ambil dari asset file agar terpasang global
    [SerializeField] Material mechaMaterial; //ambil dari asset file agar terpasang global

    public void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        GameMaster = FindAnyObjectByType<GameMaster>();
        CameraAct = FindAnyObjectByType<CameraActive>();
        rotationSpeed = CameraAct.rotationSpeed;
        Weapon = GetComponentInChildren<WeaponRaycast>();
        Mecha = GetComponent<MechaPlayer>();
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        cameraEffect = FindAnyObjectByType<CameraEffect>();
        combatVoiceAct = GetComponent<CombatVoiceActive>();
        cutSceneManager = FindFirstObjectByType<CutSceneManager>();
    }
    private void Start()
    {
        smokeVFX.SetActive(false);
        playerPosition = GetComponent<Transform>();
        skill2HitBox.SetActive(false);
        ultimateObj.SetActive(false);
        cameraPivot = CameraAct.cameraPivot;
        wasAiming = false;
        defaultSpeed = speed;
        Mecha.skill1Time = Mecha.cooldownSkill1;
        Mecha.skill2Bar = 0;
        Mecha.Ultimate = Mecha.MinUltimate;
        Mecha.UltimateRegen = false;
        Mecha.EnergyRegen = false;
        skillBusy = false;
        Mecha.Health = Mecha.MaxHealth;
        defaultDefence = Mecha.Defence;
        playerSkillObjDash.SetActive(false);

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
        interaction = gameInput.actions.FindAction("Interaction");
        reloadAction = gameInput.actions.FindAction("Reload");
        boostAction = gameInput.actions.FindAction("Boost");
        awakeningAction = gameInput.actions.FindAction("Awakening");
        resetCameraAction = gameInput.actions.FindAction("CameraReset");

        defaultUltDamage = Mecha.UltDamage;
        defaultAttack = Mecha.AttackPow;
        defaultSpeed = Mecha.defaultSpeed;
        dashSpeed = speed * 2; //DashMovement
        dashBoostSpeed = 2 * normalBoostSpeed; //BoostSpeed Effect
    }
    void Update()
    {
        PlayerDataMonitor();
        UltimateSetting();
        PauseControl();

        //Hukum Fisika COY
        ApplyGravity();
        SelectButtonPress();
        //Death();
        UpdatePosition();

        if (!Mecha.isDeath && !GameMaster.isPaused)
        {
            SKillCooldown();
            AwakeningReady();

            if (!Mecha.UsingUltimate)
            {
                //Class untuk player
                if (!Mecha.usingSkill1 && !Mecha.usingSkill2 && !Mecha.undefeat)
                {
                    StartCoroutine(ResetCameraTrigger());
                    PlayerJump();
                    Reloading();
                    ScopeMode();
                    Shooting();
                    DashPlayer();
                    BlockPlayer();
                    RelativeMovement();
                }
                //StartCoroutine(Skill1());
                if (!Mecha.isAiming && !Mecha.isShooting)
                {
                    Skill1();
                    Skill2();
                    StartCoroutine(UseUltimate());
                }
                StartCoroutine(AwakeningActive());
                //Ultimate Regen  
                //if (!Mecha.UltimateRegen && Mecha.Ultimate < Mecha.MaxUltimate && !Mecha.UltimateReady) _ = StartCoroutine(UltimateRegen());
            }
            //StartCoroutine(BoostOn());

            //EnergyRegen         
            if (!Mecha.EnergyRegen && Mecha.Energy < Mecha.MaxEnergy) _ = StartCoroutine(EnergyRegen());
        }

        //Hovering();
        SkillBusy();
        ParticleSet();
        //BoostDirectionSet();
        VisualEffect();
    }

    private float currentThrustValue = 0f;
    private float currentMiniThrust = 0f;
    private float currentEmmission = 0f;

    [SerializeField] private Color baseEmissionColor;

    void PlayerDataMonitor()
    {
        if (Mecha.Health >= Mecha.MaxHealth)
        {
            Mecha.Health = Mecha.MaxHealth;
        }
    }
    void VisualEffect()
    {
        float targetThrust = 0.7f;
        float targetMiniThrust = 0.5f;
        float emmisionValue = 0f;

        if (Mecha.UsingAwakening)
        {
            emmisionValue = 100f;
        }

        if (Mecha.isDashing)
        {
            if (Mecha.isBoosting)
            {
                targetThrust = 0.25f;
                targetMiniThrust = 0.9f;
            }
            else
            {
                targetThrust = 0.5f;
                targetMiniThrust = 0.7f;
            }
        }
        else if (Mecha.isBoosting)
        {
            targetThrust = 0.25f;
            targetMiniThrust = 0.9f;
        }

        currentThrustValue = Mathf.MoveTowards(currentThrustValue, targetThrust, Time.deltaTime * 0.5f);
        currentMiniThrust = Mathf.MoveTowards(currentMiniThrust, targetMiniThrust, Time.deltaTime * 0.5f);
        currentEmmission = Mathf.MoveTowards(currentEmmission, emmisionValue, Time.deltaTime * 15f);

        Color emission = baseEmissionColor * currentEmmission;
        mechaMaterial.SetColor("_EmissionColor", emission);
        mechaMaterial.EnableKeyword("_EMISSION");

        thusterJetVFX.SetFloat("_Thrust", currentThrustValue);
        miniThrusterVFX.SetFloat("_Thrust", currentMiniThrust);
    }

    void AwakeningReady()
    {
        if (Mecha.Awakening >= Mecha.MaxAwakening)
        {
            Mecha.awakeningReady = true;
        }
        else
        {
            Mecha.awakeningReady = false;
        }
    }

    IEnumerator AwakeningActive()
    {
        if (awakeningAction.triggered && Mecha.awakeningReady)
        {
            Mecha.awakeningReady = false;
            anim.SetTrigger("IsAwakening");
            Mecha.Awakening = Mecha.MinAwakening;
            Mecha.UsingAwakening = true;
            yield return new WaitForSeconds(Mecha.AwakeningDuration);
            Mecha.UsingAwakening = false;
        }

        //condition
        if (Mecha.UsingAwakening)
        {
            Mecha.AttackPow = Mecha.awakeningAttack;
            Mecha.UltDamage = Mecha.awakeningAttack + 200;
            Mecha.Defence = 3000;

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.72f);

            Time.timeScale = 1f;
            if (GameMaster.isPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else
        {
            Mecha.Defence = defaultDefence;
            if (!Mecha.isAttackUp)
            {
                Mecha.UltDamage = defaultUltDamage;
                Mecha.AttackPow = defaultAttack;
            }
        }
    }

    //panggil di animstate awaken / trigger event

    public void EnableAwakenCamera()
    {
        Quaternion targetRotation = playerPosition.rotation;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        CameraAct.cameraPivot.transform.rotation = targetRotation;
        mechaInAwakenState = true;
    }

    public void DisableAwakenCamera()
    {
        CameraAct.MainCamera.transform.SetLocalPositionAndRotation(new(1.29199994f,1.86699998f,-2.71099973f), Quaternion.Euler(0f, 0f, 0f));
        mechaInAwakenState = false;
    }

    private float cameraResetSpeed = 40f;
    float cameraResetTime = 0.2f;
    IEnumerator ResetCameraTrigger()
    {
        float time = 0f;
        Quaternion targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        if (resetCameraAction.triggered)
        {
            while (time < cameraResetTime)
            {
                time += Time.deltaTime;
                CameraAct.cameraPivot.transform.rotation = Quaternion.Slerp(CameraAct.cameraPivot.transform.rotation, targetRotation, cameraResetSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void DashPlayer()
    {
        if (dashAction.IsPressed() && !Mecha.isDashing && !Mecha.isBoosting)
        {
            reloadAction.Disable();
            boostAction.Disable();
            Mecha.isDashing = true;
            speed = dashSpeed;
            Debug.Log("Dash");
        }

        if (!dashAction.IsPressed() && Mecha.isDashing && !Mecha.isBoosting)
        {
            reloadAction.Enable();
            boostAction.Enable();
            Mecha.isDashing = false;
            anim.SetFloat("Move", 0f);
            speed = defaultSpeed;
        }
    }

    public IEnumerator BoostOn()
    {
        float time = 0f;
        Vector3 forward = playerPosition.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 moveDirection = (forward * boostDistance).normalized;

        if (Mecha.isBoosting && Mecha.Energy >= Mecha.EnergyCost)
        {
            playerPosition.rotation = CameraAct.MainCamera.transform.rotation;
            skillBusy = true;
            anim.SetFloat("Move", 3f);
            speed = 14f;
            Mecha.Energy -= Mecha.EnergyCost;
            while (time < boostDuration)
            {
                time += Time.deltaTime / boostDirectionLerp;
                controller.Move(speed * Time.deltaTime * moveDirection); // Gerakan bertahap 
                windEffect.SetActive(true);
                yield return null;
            }
        }
        // boost selesai
        yield return new WaitForSeconds(0.05f);
        windEffect.SetActive(false);
        skillBusy = false;
        speed = defaultSpeed;
        Mecha.isBoosting = false;
    }

    private void ParticleSet()
    {
        var particleMain = thusterParticle.main;
        //if (Mecha.isBoosting)
        //{
        //    particleMain.simulationSpeed = fastBoostSpeed;
        //}
        if (Mecha.isDashing)
        {
            particleMain.simulationSpeed = dashBoostSpeed;
        }
        else
        {
            particleMain.simulationSpeed = normalBoostSpeed;
        }

        if (Mecha.isAiming)
        {
            particleMain.simulationSpace = ParticleSystemSimulationSpace.Local;
        }
        else
        {
            particleMain.simulationSpace = ParticleSystemSimulationSpace.World;
        }
    }

    public void Reloading()
    {
        if (reloadAction.triggered || Weapon.ammo <= 0)
        {
            Mecha.isReloading = true;
        }
        else
        {
            Mecha.isReloading = false;
        }

        if (Mecha.isReloading)
        {
            anim.SetBool("IsReloading", true);
            scopeAction.Disable();
            shootAction.Disable();
            StartCoroutine(Weapon.ReloadAmmo());
            StopCoroutine(Weapon.FireShoot());
            dashAction.Disable();
            blockAction.Disable();
            skill1Action.Disable();
            skill2Action.Disable();
        }
        else
        {
            anim.SetBool("IsReloading", false);
            scopeAction.Enable();
            shootAction.Enable();
            StopCoroutine(Weapon.ReloadAmmo());
            dashAction.Enable();
            reloadAction.Enable();
            blockAction.Enable();
            skill1Action.Enable();
            skill2Action.Enable();
        }
    }
    public void ScopeMode()
    {
        CameraAct.ScopeCamera();
        if (scopeAction.IsPressed() && Weapon.ammo >= 0 && !Mecha.isDeath)
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
            dashAction.Disable();
            anim.SetBool("IsAiming", true);
            //CameraAct.ScopeCamera();
            Debug.Log("Scope On");
        }
        else
        {
            dashAction.Enable();
            anim.SetBool("IsAiming", false);
            //CameraAct.ScopeCamera();
        }

        if (Mecha.isAiming != wasAiming)
        {
            if (Mecha.isAiming)
            {
                CameraAct.rotationSpeed /= 4f;
                CameraAct.SameRotation();
            }
            else
            {
                CameraAct.rotationSpeed *= 4f;
            }
        }
        wasAiming = Mecha.isAiming;
    }
    public void Shooting()
    {
        if (shootAction.IsPressed() || scopeAction.IsPressed())
        {
            anim.SetBool("ShootingStance", true);
        }
        else
        {
            anim.SetBool("ShootingStance", false);
        }

        //Shooting
        if (shootAction.IsPressed() && Weapon.ammo >= 0 && !Mecha.isDeath)
        {
            Mecha.isShooting = true;
        }
        else
        {
            Mecha.isShooting = false;
        }

        if (Mecha.isShooting)
        {
            Debug.Log("Shoot ON");
            anim.SetBool("IsShooting", true);
            StartCoroutine(Weapon.FireShoot());
            if (!Mecha.isAiming)
            {
                CameraAct.ShootingCamera();
            }
        }
        else
        {
            anim.SetBool("IsShooting", false);
        }
    }

    private Vector3 currentMoveDirection = Vector3.zero;
    public void RelativeMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        if (!Mecha.isDeath)
        {
            if (moveInput != Vector2.zero && !Mecha.isBoosting)
            {
                trailDust.SetActive(true);
                Mecha.isIdle = false;
                Vector3 forward = cameraPivot.transform.forward;
                Vector3 right = cameraPivot.transform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();
                Vector3 targetDirection = (forward * moveInput.y + right * moveInput.x).normalized;

                currentMoveDirection = Vector3.Slerp(currentMoveDirection, targetDirection, Time.deltaTime * walkRotValue);
                if (!Mecha.isAiming && currentMoveDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                if (Mecha.isShooting && !Mecha.isAiming || Mecha.isBlocking)
                {
                    CameraAct.SameRotation();
                    Vector3 fixedRotation = transform.localEulerAngles;
                    fixedRotation.x = 0f;
                    transform.localEulerAngles = fixedRotation;
                }
                controller.Move(speed * Time.deltaTime * targetDirection); // Gerakkan player relatif terhadap kamera
                anim.SetBool("IsMove", true);
                anim.SetFloat("Move", 1f);

                if (Mecha.isDashing)
                {
                    anim.SetFloat("Move", 2f);
                }
            }
            else
            {
                Mecha.isIdle = true;
                trailDust.SetActive(false);
                if (!Mecha.isBoosting)
                {
                    anim.SetFloat("Move", 0f);
                    anim.SetBool("IsMove", false);
                }
            }

            if (boostAction.triggered && Mecha.Energy >= Mecha.EnergyCost && !Mecha.isBoosting && !Mecha.isReloading)
            {
                Mecha.isBoosting = true;
                anim.SetFloat("Move", 3f);
                StartCoroutine(BoostOn());
            }
        }
    }

    private bool wasGrounded = false;
    public void PlayerJump()
    {
        if (jumpAction.triggered && isGrounded && !Mecha.isBoosting && !Mecha.isBlocking)
        {
            Mecha.isJumping = true;
            anim.SetBool("IsJump", true);
            wasGrounded = true;
            Instantiate(jumpDust, transform.position, Quaternion.identity);
            Debug.Log("Jump");
            verticalVelocity = jumpForce;
            Vector3 jumpMovement = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            controller.Move(jumpMovement);
            isGrounded = false;
        }
        if (isGrounded && wasGrounded)
        {
            anim.SetBool("IsJump", false);
            wasGrounded = false;
            Instantiate(jumpDust, transform.position, Quaternion.identity);
            Mecha.isJumping = false;
        }
    }
    public void ApplyGravity()
    {
        if (!isGrounded)
        {
            anim.SetBool("IsGrounded", false);
            verticalVelocity -= gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            anim.SetBool("IsGrounded", true);
            verticalVelocity -= gravity * Time.deltaTime;
        }

        Vector3 gravityMovement = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        controller.Move(gravityMovement);

        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
    }

    public void BlockPlayer()
    {
        if (blockAction.IsPressed() && !Mecha.isBlocking)
        {
            Mecha.isBlocking = true;
            anim.SetBool("IsBlocking", true);
            speed /= 2;
            Debug.Log("Block");
        }

        if (!blockAction.IsPressed() && Mecha.isBlocking)
        {
            Mecha.isBlocking = false;
            anim.SetBool("IsBlocking", false);
            speed = defaultSpeed;
        }

        if (Mecha.isBlocking)
        {
            CameraAct.SameRotation();
            shieldObj.SetActive(true);
        }
        else
        {
            shieldObj.SetActive(false);
        }
    }
    public void SelectButtonPress()
    {
        if (selectButton.triggered)
        {
            Debug.Log("SelectButton");
        }
    }

    #region PlayerSkill
    void Skill1()
    {
        if (skill1Action.triggered && Mecha.readySkill1 && !Mecha.isDeath && !Mecha.isReloading && !Mecha.isBlocking)
        {
            anim.SetTrigger("IsSkill1");
            StartCoroutine(Skill1Dash());
        }

        if (Mecha.usingSkill1)
        {
            Mecha.readySkill1 = false;
            Mecha.skill1Time = Mecha.cooldownSkill1;
            skill2Action.Disable();
        }
        else
        {
            skill2Action.Enable();
        }
    }

    public void EnableDashDamage()
    {
        playerSkillObjDash.SetActive(true);
    }

    public void DisableDashDamage()
    {
        playerSkillObjDash.SetActive(false);
    }

    //panggil sekali di skill
    IEnumerator Skill1Dash()
    {
        float dashSkillTime = 0.7f;
        float skill1Distance = 2f;
        float speedDash = 20f;
        //Proses
        float time = 0f;
        Vector3 forward = playerPosition.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 moveDirection = (forward * skill1Distance).normalized;
        while (time < dashSkillTime)
        {
            time += Time.deltaTime;
            controller.Move(speedDash * Time.deltaTime * moveDirection);
            yield return null;
        }
    }

    public void SpawnSlashEffect()
    {
        Instantiate(slashEffect, WeaponSlot.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
        Instantiate(playerSkillObj, playerSkillSpawn.position, Quaternion.Euler(0f, transform.eulerAngles.y, 0f));
    }
    #endregion

    #region WeaponSkill
    void Skill2()
    {
        if (skill2Action.triggered && Mecha.readySkill2 && !Mecha.isDeath && !Mecha.isReloading && !Mecha.isBlocking)
        {
            anim.SetTrigger("IsSkill2");
            StartCoroutine(Skill2Dash());
        }

        if (Mecha.usingSkill2)
        {
            Mecha.readySkill2 = false;
            Mecha.skill2Bar = 0;
            skill1Action.Disable();
        }
        else
        {
            skill1Action.Enable();
        }
    }

    //panggil sekali di skill
    IEnumerator Skill2Dash()
    {
        float dashSkillTime = 0.7f;
        float skill2Distance = 3f;
        float speedDash = 10f;
        //Proses
        float time = 0f;
        Vector3 forward = playerPosition.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 moveDirection = (forward * skill2Distance).normalized;
        while (time < dashSkillTime)
        {
            time += Time.deltaTime;
            controller.Move(speedDash * Time.deltaTime * moveDirection);
            yield return null;
        }
    }

    //panggil di Animation State
    public void Skill2EnableVFX()
    {
        weaponSkillObj.SetActive(true);
    }

    public void Skill2DisableVFX()
    {
        weaponSkillObj.SetActive(false);
    }

    public void EnableSkill2HitBox()
    {
        skill2HitBox.SetActive(true);
    }

    public void DisableSkill2HitBox()
    {
        skill2HitBox.SetActive(false);
    }

    #endregion

    public void Death()
    {
        if (Mecha.Health <= Mecha.MinHealth)
        {
            Mecha.Health = Mecha.MinHealth;
            Mecha.isDeath = true;

            if (Mecha.isDeath)
            {
                Time.timeScale = 0.5f;
                ExplodeDeath();
                speed = 0f;
                anim.SetBool("IsDeath", true);
                GameMaster.gameLose = true;
                GameMaster.gameFinish = true;
            }
            else
            {
                anim.SetBool("IsDeath", false);
            }
        }
    }

    bool exploded = false;
    void ExplodeDeath()
    {
        if (!exploded)
        {
            exploded = true;
            Instantiate(explodedVFX, transform.position, Quaternion.identity);
            smokeVFX.SetActive(true);
        }
    }

    public void SKillCooldown()
    {
        Mecha.skill1Time -= (1 * Time.deltaTime);
        //Mecha.skill2Time -= (1 * Time.deltaTime);
        if (Mecha.skill1Time <= 0)
        {
            Mecha.skill1Time = 0;
            Mecha.readySkill1 = true;
        }

        if (Mecha.skill2Bar >= Mecha.skill2MaxBar)
        {
            Mecha.skill2Bar = Mecha.skill2MaxBar;
            Mecha.readySkill2 = true;
        }

        //if (Mecha.skill2Time <= 0)
        //{
        //    Mecha.skill2Time = 0;
        //    Mecha.readySkill2 = true;
        //}

        //skill Overlap
        if (Mecha.usingSkill1)
        {
            skill2Action.Disable();
        }
        else
        {
            skill2Action.Enable();
        }

        if (Mecha.usingSkill2)
        {
            skill1Action.Disable();
        }
        else
        {
            skill1Action.Enable();
        }
    }

    #region Ultimate
    //Ultimate
    public IEnumerator UseUltimate()
    {
        if (ultimateAction.triggered && Mecha.Ultimate >= Mecha.MaxUltimate && !Mecha.isDeath)
        {
            Quaternion targetRotation = CameraAct.MainCameraOBJ.transform.rotation;
            targetRotation.x = 0;
            targetRotation.z = 0;
            playerPosition.rotation = targetRotation;
            //Mecha.UltimateRegen = false;
            Mecha.Ultimate = Mecha.MinUltimate;
            Mecha.UsingUltimate = true;
            yield return new WaitForSeconds(Mecha.UltDuration); //lama ultimate

            Mecha.UsingUltimate = false;
        }
    }

    //Ultimate Animation Trigger
    public IEnumerator MoveBackUltimate()
    {
        if (!Mecha.UsingUltimate) yield break;

        float dashSkillTime = 5f;
        float distance = 4f;
        float speedDash = 0.2f;
        //Proses
        float time = 0f;
        Vector3 forward = playerPosition.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 moveDirection = (-forward * distance).normalized;
        while (time < dashSkillTime)
        {
            time += Time.deltaTime;
            controller.Move(speedDash * Time.deltaTime * moveDirection);
            yield return null;
        }
    }

    public void EnableUltimateBox()
    {
        ultimateObj.SetActive(true);
    }

    public void DisableUltimateBox()
    {
        ultimateObj.SetActive(false);
    }

    public void UltimateSetting()
    {
        if (Mecha.Ultimate >= Mecha.MaxUltimate)
        {
            Mecha.UltimateReady = true;
        }
        else
        {
            Mecha.UltimateReady = false;
        }

        if (Mecha.Ultimate >= Mecha.MaxUltimate)
        {
            Mecha.Ultimate = Mecha.MaxUltimate;
        }

        if (Mecha.UsingUltimate)
        {
            anim.SetBool("IsUltimate", true);
            Mecha.Ultimate = Mecha.MinUltimate;
            //StopCoroutine(UltimateRegen());
        }
        else
        {
            anim.SetBool("IsUltimate", false);
        }
    }
    #endregion

    //public IEnumerator UltimateRegen()
    //{
    //    Mecha.UltimateRegen = true;
    //    while (Mecha.Ultimate <= Mecha.MaxUltimate)
    //    {
    //        if (Mecha.UsingUltimate)
    //        {
    //            Mecha.UltimateRegen = false;
    //            yield break;
    //        }
    //        yield return new WaitForSeconds(1f);
    //        Mecha.Ultimate += Mecha.UltRegenValue;
    //        Mecha.Ultimate = Mathf.Clamp(Mecha.Ultimate, Mecha.MinUltimate, Mecha.MaxUltimate);
    //    }
    //    Mecha.UltimateRegen = false;
    //}

    //Energy
    public IEnumerator EnergyRegen()
    {
        Mecha.EnergyRegen = true;
        while (Mecha.Energy <= Mecha.MaxEnergy)
        {
            yield return new WaitForSeconds(1f);
            Mecha.Energy += Mecha.EngRegenValue;
            Mecha.Energy = Mathf.Clamp(Mecha.Energy, Mecha.MinEnergy, Mecha.MaxEnergy);
        }
    }

    public void SkillBusy()
    {
        if (skillBusy || Mecha.isBoosting || Mecha.isBlocking || Mecha.isJumping)
        {
            reloadAction.Disable();
            shootAction.Disable();
            scopeAction.Disable();
            dashAction.Disable();
            boostAction.Disable();
            ultimateAction.Disable();
            skill1Action.Disable();
            skill2Action.Disable();
        }
        else
        {
            reloadAction.Enable();
            shootAction.Enable();
            scopeAction.Enable();
            dashAction.Enable();
            boostAction.Enable();
            ultimateAction.Enable();
        }
    }

    public void PauseControl()
    {
        if (GameMaster.isPaused || cutSceneManager.isPlaying)
        {
            skillBusy = true;
            reloadAction.Disable();
            shootAction.Disable();
            scopeAction.Disable();
            dashAction.Disable();
            boostAction.Disable();
            ultimateAction.Disable();
            jumpAction.Disable();
            skill1Action.Disable();
            skill2Action.Disable();
            awakeningAction.Disable();
        }
        else
        {
            skillBusy = false;
            reloadAction.Enable();
            shootAction.Enable();
            scopeAction.Enable();
            dashAction.Enable();
            boostAction.Enable();
            ultimateAction.Enable();
            jumpAction.Enable();
            awakeningAction.Enable();
        }
    }

    public void TakeDamage(int damage)
    {
        int damageCal = damage - Mecha.Defence;
        if (!Mecha.UsingUltimate || Mecha.undefeat)
        {
            combatVoiceAct.DamageVoice();
            Mecha.Health -= damageCal;
            hitSound.Play();
            StartCoroutine(cameraEffect.HitEffect());
            Debug.Log("Player Damage " + damageCal);
        }
    }
    public void UpdatePosition()
    {
        Mecha.PlayerPosition = playerPosition;
    }
}