using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private Transform player;
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
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponent<Animator>();
        if (enemyModel == null) enemyModel = GetComponent<EnemyModel>();
        if (enemyActive == null) enemyActive = GetComponent<EnemyActive>();
        
        {
            
        }

        if (atackPoint == null)
        {
            atackPoint = new GameObject("AttackPoint").transform;
            atackPoint.SetParent(transform);
            atackPoint.localPosition = new Vector3(0, 1f, 1f);
        }
    }

    private void Update()
    {

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
        StartCoroutine(ApplyDamageDelayed(0.2f));
    }

    private IEnumerator ApplyDamageDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Meng aplikasikan damage ke player jika di dalam range
        if(enemyActive.player != null)
        {
            float distanceToPlayer = Vector3.Distance(atackPoint.position, enemyActive.player.position);    

            if(distanceToPlayer <= attackRadius)
            {
                // Kalkulasi Damage - bisa sangat 
                int damage = enemyModel.attackPower;
                if (currentComboIndex > 0) damage = Mathf.RoundToInt(damage * (1f + currentComboIndex * 0.2f));

                // Aplikasikan damage
                enemyActive.player.GetComponent<PlayerActive>()?.TakeDamage(damage);

                Debug.Log($"Sword attack hit player for {damage} damage (combo: {currentComboIndex})");
            }
        }
        StartCoroutine(ResetAttackState(0.5f));

    }

    private IEnumerator ResetAttackState(float delay)
    {
        yield return new WaitForSeconds (delay);
        enemyModel.isAttacking = false;
    }

    public void OnAttackHit()
    {
        if(enemyActive.player != null)
        {
            float distanceToPlayer = Vector3.Distance(atackPoint.position, enemyActive.player.position);
            if(distanceToPlayer <= attackRadius)
            {
                int damage = enemyModel.attackPower;
                if (currentComboIndex > 0) damage = Mathf.RoundToInt(damage * (1f + currentComboIndex * 0.2f));

                // Aplikasikan damage
                enemyActive.player.GetComponent<PlayerActive>()?.TakeDamage(damage);

                Debug.Log($"Sword attack hit player for {damage} damage (combo: {currentComboIndex})");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(atackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(atackPoint.position, attackRadius);
        }
    }

}
