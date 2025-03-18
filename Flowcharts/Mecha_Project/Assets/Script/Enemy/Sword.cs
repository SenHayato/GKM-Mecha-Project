using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyModel enemyModel;
    [SerializeField] private EnemyActive enemyActive;

    [Header("Attack Configuration")]
    [SerializeField] private int comboLength = 3;
    [SerializeField] private float comboTimeWindow = 2f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private Transform atackPoint;

    [Header("Attack Patterns")]
    [SerializeField] private bool useRandomPattern = false;
    [SerializeField] private string[] attackTriggers = { "Attack 1", "Attack 2", "Attack 3"};

    private int currentComboIndex = 0;
    private float lastAttackTime = 0f;
    private bool isInCombo = false;

    private void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (enemyModel == null) enemyModel = GetComponent<EnemyModel>();
        if (enemyActive == null) enemyActive = GetComponent<EnemyActive>();

        if (atackPoint == null)
        {
            atackPoint = new GameObject("AttackPoint").transform;
            atackPoint.SetParent(transform);
            atackPoint.localPosition = new Vector3(0, 1f, 1f);
        }
    }

    public void PerformAttackShort()
    {
        if (enemyModel.isAttacking || enemyModel.isDeath) return;

        // Cek jika enemy harus memulai combo baru atau combo yang sudah ada
        if (Time.time - lastAttackTime > comboTimeWindow)
        {
            currentComboIndex = 0;
            isInCombo = true;
        }
        else if (currentComboIndex > -comboLength - 1)
        {
            // Combo telah berakhir, mulai kembali
            currentComboIndex = 0;
        }
        else
        {
            // Melanjutkan combo
            currentComboIndex++;
        }

        // Melakukan penyerangan yang asli
        string attackTrigger = useRandomPattern ?
            attackTriggers[Random.Range(0, attackTriggers.Length)] : attackTriggers[currentComboIndex % attackTriggers.Length];

        // Animasi akan ter trigger
        anim.SetTrigger(attackTrigger);
        enemyModel.isAttacking = true;

        // Merekan sewaktu menyerang
        lastAttackTime = Time.time;

        // Damage masuk dalam animasi acara atau langsung
        //StartCoroutine(ApplyDamageDelayed(0.2f));
    }
}
