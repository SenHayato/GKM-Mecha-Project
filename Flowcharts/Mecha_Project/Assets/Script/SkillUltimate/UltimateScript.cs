using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UltimateScript : MonoBehaviour
{
    [SerializeField] MechaPlayer playerData;
    [SerializeField] PlayerActive playerActive;
    [SerializeField] float damageRadius;

    private Collider[] enemyColliders;

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

    private void OnEnable()
    {
        enemyColliders = Physics.OverlapSphere(transform.position, damageRadius, playerActive.enemyLayer);

        foreach (var hitCollider in enemyColliders)
        {
            if (hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
            {
                enemyActive.enemyModel.isStunt = true;
                StartCoroutine(ApplyDamageOverTime(enemyActive));
            }
        }
    }

    private void OnDisable()
    {
        foreach (var hitCollider in enemyColliders)
        {
            if (hitCollider.TryGetComponent<EnemyActive>(out var enemyActive))
            {
                StopCoroutine(ApplyDamageOverTime(enemyActive));
                if (enemyActive != null)
                {
                    enemyActive.enemyModel.isStunt = false;
                }
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(EnemyActive enemy)
    {
        for (float t = 0; t < duration; t += interval)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(playerData.UltDamage);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
