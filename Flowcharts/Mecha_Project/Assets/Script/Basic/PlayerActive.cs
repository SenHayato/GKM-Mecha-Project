using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActive : MonoBehaviour
{
    [SerializeField] PlayerInput gameInput;

    // Input
    public InputAction moveAction, jumpAction, flyUp, shootAction, scopeAction, skill1Action, skill2Action,
        flyDown, blockAction, dashAction, selectButton, ultimateAction, interaction, reloadAction, boostAction, awakeningAction;
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
    //public WeaponScript weapon;
    public WeaponRaycast Weapon;
    public ParticleSystem thusterParticle;
    public GameObject windEffect;
    public HashSet<string> enemyTags = new() { "Enemy", "Boss", "MiniBoss" };
    //public Material[] playerMaterial; //Material yanhg bisa berubah warna
    //private Color[] defaultColor;

    [Header("Default Parameter")]
    private float defaultSpeed;
    private float dashSpeed;
    private int defaultAttack;
    private int defaultUltDamage;

    [Header("HitBox")]
    public GameObject skill1HitBox;
    public GameObject skill2HitBox;
    public GameObject ultimateHitBox;

    [Header("BoostEffect")]
    [SerializeField] private float normalBoostSpeed;
    [SerializeField] private float fastBoostSpeed;
    [SerializeField] private float dashBoostSpeed;

    [Header("Mecha Sound")]
    [SerializeField] AudioSource hitSound;
    public AudioSource thrusterSound;
    [SerializeField] AudioSource blockSound;

    [Header("Player Status")]
    public float speed;
    public float jumpForce;
    public float gravity;
    public bool isGrounded;
    public float verticalVelocity;
    public float fallMultiplier;
    public float rotationSpeed;
    private bool wasAiming;
    [SerializeField] bool skillBusy;
    public CharacterController controller;
    public Animator anim;

    [Header("Boost Config")]
    public float boostDuration;
    public float boostSpeedMultiplier;
    [SerializeField] float boostDistance;
    [SerializeField] float boostDirectionLerp = 0;

    public void Awake()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        gameInput = FindAnyObjectByType<PlayerInput>();
        GameMaster = FindAnyObjectByType<GameMaster>();
        CameraAct = FindAnyObjectByType<CameraActive>();
        rotationSpeed = CameraAct.rotationSpeed;
        //skill1HitBox = GameObject.Find("Skill1HitBox");
        //skill1HitBox = GameObject.Find("Skill2HitBox");
        Weapon = GetComponentInChildren<WeaponRaycast>();
        Mecha = GetComponent<MechaPlayer>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cameraEffect = FindAnyObjectByType<CameraEffect>();
        combatVoiceAct = GetComponent<CombatVoiceActive>();
        cutSceneManager = FindFirstObjectByType<CutSceneManager>();
    }
    private void Start()
    {
        playerPosition = GetComponent<Transform>();
        //cameraPivot = CameraAct.cameraParent;
        skill1HitBox.SetActive(false);
        skill2HitBox.SetActive(false);
        ultimateHitBox.SetActive(false);
        cameraPivot = CameraAct.cameraPivot;
        wasAiming = false;
        defaultSpeed = speed;
        //Mecha Skill dan Ultimate Condition
        Mecha.skill1Time = Mecha.cooldownSkill1;
        Mecha.skill2Time = Mecha.cooldownSkill2;
        Mecha.Ultimate = Mecha.MinUltimate;
        //Mecha.Energy = Mecha.MaxEnergy;
        Mecha.UltimateRegen = false;
        Mecha.EnergyRegen = false;
        skillBusy = false;

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
        
        defaultUltDamage = Mecha.UltDamage;
        defaultAttack = Mecha.AttackPow;
        defaultSpeed = Mecha.defaultSpeed;
        dashSpeed = speed * 2; //DashMovement
        dashBoostSpeed = 2 * normalBoostSpeed; //BoostSpeed Effect
    }
    void Update()
    {
        PauseControl();

        //Hukum Fisika COY
        ApplyGravity();
        SelectButtonPress();

        //Class untuk player
        PlayerJump();
        Reloading();
        DashPlayer();
        BlockPlayer();
        ScopeMode();
        Shooting();
        //Hovering();
        Death();
        RelativeMovement();
        StartCoroutine(Skill1());
        StartCoroutine(Skill2());
        UpdatePosition();
        SKillCooldown();
        StartCoroutine(UseUltimate());
        StartCoroutine(AwakeningActive());
        AwakeningReady();
        //StartCoroutine(BoostOn());
        //Ultimate Energy Regen
        if (!Mecha.UltimateRegen && Mecha.Ultimate < Mecha.MaxUltimate) _= StartCoroutine(UltimateRegen());
        if (!Mecha.EnergyRegen && Mecha.Energy < Mecha.MaxEnergy) _= StartCoroutine(EnergyRegen());

        SkillBusy();
        ParticleSet();
        //BoostDirectionSet();
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
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(0.72f);
            Time.timeScale = 1;
            if (GameMaster.isPaused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1f;
            }
            //foreach (var materials in playerMaterial)
            //{
            //    materials.color = Color.blue;
            //}
        }
        else
        {
            if (!Mecha.isAttackUp)
            {
                Mecha.UltDamage = defaultUltDamage;
                Mecha.AttackPow = defaultAttack;
            }
            //foreach (var materials in playerMaterial)
            //{
            //    materials.color = Color.white;
            //}
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
        Vector3 forward = cameraPivot.transform.forward;
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
            while (time < 1f)
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
        if (Mecha.isBoosting)
        {
            particleMain.simulationSpeed = fastBoostSpeed;
        }
        else if (Mecha.isDashing)
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
    public void RelativeMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        if (!Mecha.isDeath)
        {
            if (moveInput != Vector2.zero && !Mecha.isBoosting)
            {
                Mecha.isIdle = false;
                Vector3 forward = cameraPivot.transform.forward;  // Arah kamera
                Vector3 right = cameraPivot.transform.right;
                forward.y = 0f; // Tetap Horizontal
                right.y = 0f;
                forward.Normalize();
                right.Normalize();
                Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized; // Hitung arah pergerakan relatif terhadap kamera

                if (!Mecha.isAiming)
                {
                    // Rotasi player menghadap arah pergerakan
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                if (Mecha.isShooting && !Mecha.isAiming) //temp
                {
                    CameraAct.SameRotation();
                    Vector3 fixedRotation = transform.localEulerAngles;
                    fixedRotation.x = 0f;
                    transform.localEulerAngles = fixedRotation;
                }
                controller.Move(speed * Time.deltaTime * moveDirection); // Gerakkan player relatif terhadap kamera
                anim.SetFloat("Move", 1f);
                anim.SetBool("IsMove", true);

                if (Mecha.isDashing)
                {
                    anim.SetFloat("Move", 2f);
                }
            }
            else
            {
                Mecha.isIdle = true;
                if (!Mecha.isBoosting)
                {
                    anim.SetFloat("Move", 0f);
                    anim.SetBool("IsMove", false);
                }
            }

            if (boostAction.triggered && Mecha.Energy >= Mecha.EnergyCost && !Mecha.isBoosting)
            {
                Mecha.isBoosting = true;
                anim.SetFloat("Move", 3f);
                StartCoroutine(BoostOn());
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
        if (jumpAction.triggered && isGrounded && !Mecha.isBoosting && !Mecha.isBlocking)
        {
            Mecha.isJumping = true;
            Debug.Log("Jump");
            verticalVelocity = jumpForce;
            Vector3 jumpMovement = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            controller.Move(jumpMovement);
            isGrounded = false;
            anim.SetBool("IsJump", true);
        }
        if (isGrounded)
        {
            Mecha.isJumping = false;
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
    }
    public void SelectButtonPress()
    {
        if (selectButton.triggered)
        {
            Debug.Log("SelectButton");
        }
    }

    public IEnumerator Skill1()
    {
        if (skill1Action.triggered && Mecha.readySkill1 && !Mecha.isDeath && !Mecha.isReloading && !Mecha.isBlocking)
        {
            Debug.Log("Skill 1 Aktif Korotine");
            skillBusy = true;
            Mecha.usingSkill1 = true;
            Mecha.skill1Time = Mecha.cooldownSkill1;
            skill2Action.Disable();
            Mecha.readySkill1 = false;
            anim.SetTrigger("IsSkill1");

            yield return new WaitForSeconds(1.1f);
            skill1HitBox.SetActive(true);

            yield return new WaitForSeconds(Mecha.skill1Duration); //lama skill
            Mecha.usingSkill1 = false;
            skillBusy = false;
            skill2Action.Enable();
            skill1HitBox.SetActive(false);
        }
        else
        {
            yield break;
        }
    }

    public IEnumerator Skill2()
    {
        if (skill2Action.triggered && Mecha.readySkill2 && !Mecha.isDeath && !Mecha.isReloading && !Mecha.isBlocking)
        {
            Debug.Log("Skill 2 Aktif Korotine");
            skillBusy = true;
            Mecha.usingSkill2 = true;
            Mecha.skill2Time = Mecha.cooldownSkill2;
            skill1Action.Disable();
            Mecha.readySkill2 = false;
            anim.SetTrigger("IsSkill2");

            yield return new WaitForSeconds(2f);
            skill2HitBox.SetActive(true);

            yield return new WaitForSeconds(Mecha.skill2Duration); //lama skill
            Mecha.usingSkill2 = false;
            skillBusy = false;
            skill1Action.Enable();
            skill2HitBox.SetActive(false);
        }
        else
        {
            yield break;
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
                Time.timeScale = 0.5f;
                speed = 0f;
                anim.SetBool("IsDeath", true);
                GameMaster.gameLose = true;
                GameMaster.gameFinish = true;
                //Invoke(nameof(ToLoseCG), 5f);
            }
            else
            {
                anim.SetBool("IsDeath", false);
            }
        }
    }
    public void SKillCooldown()
    {
        Mecha.skill1Time -= (1 * Time.deltaTime);
        Mecha.skill2Time -= (1 * Time.deltaTime);
        if (Mecha.skill1Time <= 0)
        {
            Mecha.skill1Time = 0;
            Mecha.readySkill1 = true;
        }

        if (Mecha.skill2Time <= 0)
        {
            Mecha.skill2Time = 0;
            Mecha.readySkill2 = true;
        }

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
    //Ultimate
    public IEnumerator UseUltimate()
    {
        if (ultimateAction.triggered && Mecha.Ultimate == Mecha.MaxUltimate && !Mecha.isDeath)
        {
            Debug.Log("Ultimate jalan");
            Mecha.Ultimate = Mecha.MinUltimate;
            Mecha.UsingUltimate = true;
            ultimateHitBox.SetActive(true);

            yield return new WaitForSeconds(Mecha.UltDuration); //lama ultimate
            ultimateHitBox.SetActive(false);
            Mecha.UltimateRegen = false;
            Mecha.UsingUltimate = false;
            Debug.Log("Ultimate berenti");
        }
    }
    public IEnumerator UltimateRegen()
    {
        Mecha.UltimateRegen = true;
        while (Mecha.Ultimate <= Mecha.MaxUltimate)
        {
            yield return new WaitForSeconds(1f);
            Mecha.Ultimate += Mecha.UltRegenValue;
            Mecha.Ultimate = Mathf.Clamp(Mecha.Ultimate, Mecha.MinUltimate, Mecha.MaxUltimate);
        }
    }
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
        if (!Mecha.isBlocking)
        {
            combatVoiceAct.DamageVoice();
            Mecha.Health -= damageCal;
            hitSound.Play();
            StartCoroutine(cameraEffect.HitEffect());
            Debug.Log("Player Damage " + damageCal);
        }
        else
        {
            blockSound.Play();
        }
    }
    public void UpdatePosition()
    {
        Mecha.PlayerPosition = playerPosition;
    }
    //public void ToLoseCG()
    //{
    //    GameMaster.LosingScreen();
    //}
}