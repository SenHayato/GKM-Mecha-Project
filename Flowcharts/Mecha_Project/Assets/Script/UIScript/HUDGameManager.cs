using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class HUDGameManager : MonoBehaviour
{
    [Header("UIAtribut")]
    [SerializeField] private TextMeshProUGUI currentAmmo;
    [SerializeField] private TextMeshProUGUI maxAmmo;
    [SerializeField] private TextMeshProUGUI healthPoint;
    [SerializeField] private TextMeshProUGUI questInfo;
    [SerializeField] private TextMeshProUGUI killCounterTxt;
    [SerializeField] private UnityEngine.UI.Image healthImage;
    [SerializeField] private UnityEngine.UI.Slider healthBar;
    [SerializeField] private UnityEngine.UI.Slider easeHealthBar;
    [SerializeField] private UnityEngine.UI.Slider ultimateBar;
    [SerializeField] private UnityEngine.UI.Slider energyBar;
    [SerializeField] private UnityEngine.UI.Slider ammoBar;
    [SerializeField] private UnityEngine.UI.Slider skill1Bar;
    [SerializeField] private UnityEngine.UI.Slider skill2Bar;
    [SerializeField] private UnityEngine.UI.Slider awakeningBar;
    public TextMeshProUGUI timerText;

    [Header("Audio Library")]
    [SerializeField] AudioClip killDing;

    [Header("Audio SetUP")]
    [SerializeField] AudioSource soundSource;
    [SerializeField] bool soundActive = false;

    [Header("CrossHair")]
    public RectTransform recoilCrossHair;
    public UnityEngine.UI.Image hitEffect;
    public GameObject scopeHair;
    //public GameObject hitCrossHair;

    [Header("MiniMAP")]
    [SerializeField] private GameObject mapCamera;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private GameObject mainCamera;

    [Header("Referensi")]
    [SerializeField] private WeaponRaycast weaponScript;
    [SerializeField] private MechaPlayer mechaScript;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameMaster gameMaster;

    [Header("Animation")]
    public Animation questUIAnim;

    //flag
    bool wasKill = false;

    void Awake()
    {
        gameMaster = FindAnyObjectByType<GameMaster>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        mechaScript = playerObj.GetComponent<MechaPlayer>();
        weaponScript = playerObj.GetComponentInChildren<WeaponRaycast>();
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerIcon = GameObject.Find("PlayerIconMap");
    }

    private void Start()
    {
        //health
        healthBar.maxValue = mechaScript.MaxHealth;
        //ultimate
        ultimateBar.maxValue = mechaScript.MaxUltimate;
        //energy
        energyBar.maxValue = mechaScript.MaxEnergy;
        //skill
        skill1Bar.maxValue = mechaScript.cooldownSkill1;
        skill2Bar.maxValue = mechaScript.cooldownSkill2;
        //ease
        easeHealthBar.maxValue = healthBar.maxValue;
        easeHealthBar.value = easeHealthBar.maxValue;
        //awakening
        awakeningBar.maxValue = mechaScript.MaxAwakening;

        hitEffect = recoilCrossHair.GetComponent<UnityEngine.UI.Image>();
        soundSource = GetComponent<AudioSource>();
    }

    public void AmmoMonitor()
    {
        ammoBar.value = weaponScript.ammo;
        ammoBar.maxValue = weaponScript.maxAmmo;
        currentAmmo.text = weaponScript.ammo.ToString();
        maxAmmo.text = weaponScript.maxAmmo.ToString();
    }

    public void BarMonitor()
    {
        //HealthBar
        healthBar.value = mechaScript.Health;
        healthPoint.text = healthBar.value.ToString();
        if (healthBar.value <= 25000)
        {
            healthImage.color = Color.red;
        }
        else
        {
            healthImage.color = Color.white;
        }
        //UltimateBar
        ultimateBar.value = mechaScript.Ultimate;
        //EnergyBar
        energyBar.value = mechaScript.Energy;
        //Skill1
        skill1Bar.value = mechaScript.skill1Time;
        //Skill2
        skill2Bar.value = mechaScript.skill2Time;
        //Awakening
        awakeningBar.value = mechaScript.Awakening;
    }

    public void EaseHealthBar()
    {
        float smoothSpeed = 0.005f; 
        if (easeHealthBar.value != healthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, healthBar.value, smoothSpeed);
        }
    }

    public void KillCounter()
    {
        killCounterTxt.text = gameMaster.KillCount.ToString();

        if (gameMaster.KillCount % 10 == 0 && gameMaster.KillCount > 0 && wasKill && soundActive) //kill count habis di bagi 10
        {
            soundSource.clip = killDing;
            wasKill = false;
            soundSource.Play();
        }

        if (gameMaster.KillCount % 10 != 0)
        {
            wasKill = true;
        }

        KillCount(); //test class
    }

    public void QuestMonitor()
    {
        questInfo.text = gameMaster.QuestText;
    }

    public void TimerSetUp()
    {
        if (gameMaster.countdown)
        {
            timerText.enabled = true;
        }
        else
        {
            timerText.enabled = false;
        }
    }

    public void MiniMap()
    {
        //float time = 0.5f;
        //playerIcon.transform.position = playerObj.transform.position;
        //playerIcon.transform.rotation = Quaternion.Euler(0f, playerObj.transform.eulerAngles.y, 0f);
        mapCamera.transform.SetPositionAndRotation(playerObj.transform.position, Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f));
        Vector3 fixedRotation = playerIcon.transform.eulerAngles;
        fixedRotation.x = 90f;
        playerIcon.transform.eulerAngles = fixedRotation;
    }

    public void RecoilCrossHair()
    {
        //UnityEngine.UI.Image scopeImage = scopeHair.GetComponent<UnityEngine.UI.Image>();
        //float smoothTime;
        //float lerpSpeed = 1f;
        if (mechaScript.isShooting)
        {
            if (!mechaScript.isAiming)
            {
                recoilCrossHair.sizeDelta = new Vector2(145f, 145f);
            }
            else
            {
                recoilCrossHair.sizeDelta = new Vector2(115f, 115f);
            }
        } else
        {
            recoilCrossHair.sizeDelta = new Vector2(88f, 88f);
        }

        if (mechaScript.isAiming)
        {
            scopeHair.SetActive(true);
        }
        else
        {
            scopeHair.SetActive(false);
        }
    }

    //test
    public void TakeDamage()
    {
        PlayerInput gameInput = FindAnyObjectByType<PlayerInput>();
        InputAction testButton = gameInput.actions.FindAction("TestAja");
        if (testButton.triggered)
        {
            mechaScript.Health -= 10000;
            Debug.Log("IzulAJG");
        }
    }

    public void KillCount()
    {
        PlayerInput gameInput = FindAnyObjectByType<PlayerInput>();
        InputAction testButton = gameInput.actions.FindAction("TestKill");
        if (testButton.triggered)
        {
            gameMaster.KillCount++;
        }
    }

    void Update()
    {
        AmmoMonitor();
        BarMonitor();
        EaseHealthBar();
        KillCounter();
        MiniMap();
        QuestMonitor();
        TimerSetUp();
        RecoilCrossHair();

        TakeDamage();
    }
}