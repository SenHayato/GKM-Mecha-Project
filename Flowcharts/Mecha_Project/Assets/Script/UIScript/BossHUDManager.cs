using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossHUDManager : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] UnityEngine.UI.Slider healthBar;
    [SerializeField] UnityEngine.UI.Slider easeBar;
    [SerializeField] TextMeshProUGUI enemyName;

    private void Awake()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
    }

    private void Start()
    {
        healthBar.maxValue = enemyModel.maxHealth;
        easeBar.maxValue = healthBar.maxValue;
        healthBar.value = enemyModel.health;
        easeBar.value = easeBar.maxValue;
        enemyName.text = enemyModel.enemyName;
    }

    void BossMonitor()
    {
        healthBar.value = enemyModel.health;
    }

    public void EaseHealthBar()
    {
        float smoothSpeed = 0.005f; 
        if (easeBar.value != healthBar.value)
        {
            easeBar.value = Mathf.Lerp(easeBar.value, healthBar.value, smoothSpeed);
        }
    }

    void Update()
    {
        BossMonitor();
        EaseHealthBar();
    }
}
