using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] float damageRadius;

    private Collider[] enemyColliders;

    private bool giveDamage = true;
    private float duration;
    private float interval;

    private void Awake()
    {
        playerData = GetComponentInParent<MechaPlayer>();
        playerActive = GetComponentInParent<PlayerActive>();
    }

    private void Start()
    {
        duration = playerData.UltDuration;
        interval = playerData.UltInterval;
    }

    private void Update()
    {
        FindEnemy();
        //GiveDamage();
    }

    void FindEnemy()
    {
        if (giveDamage)
        {
            giveDamage = false;
            enemyColliders = Physics.OverlapSphere(transform.position, damageRadius, playerActive.enemyLayer);
            GiveDamage();
            Invoke(nameof(ResetDamage), interval);
        }
    }

    void GiveDamage()
    {
        if (enemyColliders != null)
        {
            foreach (var hitCollider in enemyColliders)
            {
                if (hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
                {
                    enemyActive.enemyModel.isStunt = true;
                    enemyActive.TakeDamage(playerData.UltDamage);
                }
            }
        }
    }

    void ResetDamage()
    {
        giveDamage = true;
    }

    private void OnDisable()
    {
        giveDamage = true;

        if (enemyColliders != null)
        {
            foreach (var hitCollider in enemyColliders)
            {
                if (hitCollider != null && hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
                {
                    enemyActive.enemyModel.isStunt = false;
                }
            }

            enemyColliders = null;
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Visualisasi area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
