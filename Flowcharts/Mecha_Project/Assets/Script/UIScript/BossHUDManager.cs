using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BossHUDManager : MonoBehaviour
{
    [SerializeField] EnemyModel enemyModel;
    [SerializeField] BossActive bossActive;
    [SerializeField] UnityEngine.UI.Slider healthBar;
    [SerializeField] UnityEngine.UI.Image healthBarImage;
    [SerializeField] UnityEngine.UI.Slider easeBar;
    [SerializeField] TextMeshProUGUI enemyName;

    private void Awake()
    {
        enemyModel = GetComponentInParent<EnemyModel>();
        bossActive = GetComponentInParent<BossActive>();
    }

    private void Start()
    {
        healthBar.maxValue = enemyModel.maxHealth;
        easeBar.maxValue = healthBar.maxValue;
        healthBar.value = enemyModel.health;
        easeBar.value = easeBar.maxValue;
        enemyName.text = enemyModel.enemyName;
    }

    Color barColor;
    void BossMonitor()
    {
        healthBar.value = enemyModel.health;

        if (bossActive.SecondState)
        {
            barColor = new(0.815f, 0.878f, 0.137f);
        }
        else
        {
            barColor = Color.white;
        }

        healthBarImage.color = barColor;
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
