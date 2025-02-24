using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    public UnityEngine.UI.Slider healthBar;
    public UnityEngine.UI.Slider easeBar;
    public EnemyModel enemyData;
    public GameObject playerCamera;

    [SerializeField] private GameObject UIhealth;

    private void Awake()
    {
        enemyData = GetComponentInParent<EnemyModel>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        healthBar.maxValue = enemyData.maxHealth;
        easeBar.maxValue = enemyData.maxHealth;
        easeBar.value = easeBar.maxValue;
    }

    public void BarMonitor()
    {
        //monitor
        healthBar.value = enemyData.health;
    }

    public void EaseBar()
    {
        float smoothspeed = 0.005f;
        if (easeBar.maxValue != healthBar.value)
        {
            easeBar.value = Mathf.Lerp(easeBar.value, healthBar.value, smoothspeed);
        }
    }

    public void LookPlayer()
    {
        UIhealth.transform.LookAt(playerCamera.transform);
    }

    private void Update()
    {
         //Monitor
        BarMonitor();
        EaseBar();
        LookPlayer();
    }
}
