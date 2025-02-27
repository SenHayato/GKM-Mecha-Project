using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public TextMeshProUGUI timerText;

    [Header("MiniMAP")]
    [SerializeField] private GameObject mapCamera;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private GameObject mainCamera;

    [Header("CrossHair")]
    public RectTransform recoilCrossHair;
    public GameObject hitCrossHair;

    [Header("Referensi")]
    [SerializeField] private WeaponRaycast weaponScript;
    [SerializeField] private MechaPlayer mechaScript;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameMaster gameMaster;

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
        healthBar.maxValue = mechaScript.MaxHealth;
        ultimateBar.maxValue = mechaScript.MaxUltimate;
        energyBar.maxValue = mechaScript.MaxEnergy;
        skill1Bar.maxValue = mechaScript.cooldownSkill1;
        skill2Bar.maxValue = mechaScript.cooldownSkill2;
        easeHealthBar.maxValue = healthBar.maxValue;
        easeHealthBar.value = easeHealthBar.maxValue;

        hitCrossHair.SetActive(false);
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
    }

    public void EaseHealthBar()
    {
        float smoothSpeed = 0.005f; 
        if (easeHealthBar.value != healthBar.value)
        {
            easeHealthBar.value = Mathf.Lerp(easeHealthBar.value, healthBar.value, smoothSpeed);
        }
    }

    public void RecoilCrossHair()
    {
        if (mechaScript.isShooting)
        {
            if (mechaScript.isAiming)
            {
                recoilCrossHair.sizeDelta = new Vector2(45f, 45f);
            }
            else
            {
                recoilCrossHair.sizeDelta = new Vector2(60f, 60f);
            }
        }
        else
        {
            recoilCrossHair.sizeDelta = new Vector2(20f, 20f);
        }
    }

    public void KillCounter()
    {
        killCounterTxt.text = gameMaster.KillCount.ToString();

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